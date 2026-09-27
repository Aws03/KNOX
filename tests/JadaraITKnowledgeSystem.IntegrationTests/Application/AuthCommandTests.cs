using JadaraITKnowledgeSystem.Application.Common.Options;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Login;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Logout;
using JadaraITKnowledgeSystem.Application.Features.Auth.Commands.RefreshToken;
using JadaraITKnowledgeSystem.Application.Features.Auth.Errors;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Services.Security;
using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace JadaraITKnowledgeSystem.IntegrationTests.Application;

/// <summary>Login/refresh/logout rules with the real token store; Identity itself is substituted.</summary>
[Collection(InfrastructureCollection.Name)]
public sealed class AuthCommandTests : IAsyncLifetime
{
    private const string Password = "Correct#123";

    private readonly InfrastructureFixture _database;
    private readonly AppDbContext _context;
    private readonly IIdentityUserService _identity = Substitute.For<IIdentityUserService>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();
    private readonly RefreshTokenService _refreshTokens;
    private readonly int _identityUserId = Random.Shared.Next(1_000_000, int.MaxValue);
    private string _email = string.Empty;

    public AuthCommandTests(InfrastructureFixture database)
    {
        _database = database;
        _context = database.CreateContext();
        _refreshTokens = new RefreshTokenService(
            _context, MsOptions.Create(new JwtOptions { RefreshTokenDays = 7 }), TimeProvider.System, NullLogger<RefreshTokenService>.Instance);
        _jwt.GenerateJwtTokenAsync(default, default, default, default, default!).ReturnsForAnyArgs("access-token");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _context.DisposeAsync();

    private async Task<User> SeedLinkedUserAsync(bool verified = true, bool active = true, params string[] roles)
    {
        var (_, _, major) = await _context.SeedHierarchyAsync();
        _email = TestData.UniqueEmail();
        var user = await _context.SeedUserAsync(major.Id, _email);
        if (verified) user.VerifyAccount();
        if (!active) user.BlockAccount();
        await _context.SaveChangesAsync();

        var info = new IdentityUserInfo(_identityUserId, user.Id, "Student", _email);
        _identity.FindByEmailAsync(_email).Returns(info);
        _identity.FindByIdAsync(_identityUserId).Returns(info);
        _identity.CheckPasswordAsync(_identityUserId, Password).Returns(true);
        _identity.GetRolesAsync(_identityUserId).Returns(roles.Length > 0 ? roles : [Roles.User]);
        return user;
    }

    private AuthTokenIssuer Issuer() => new(_context, _identity, _jwt, _refreshTokens);

    private LoginCommandHandler LoginHandler(bool requireVerification = false) => new(
        _identity, _context, Issuer(),
        MsOptions.Create(new AuthOptions { RequireEmailVerification = requireVerification }),
        NullLogger<LoginCommandHandler>.Instance);

    private RefreshTokenCommandHandler RefreshHandler() =>
        new(_refreshTokens, _identity, Issuer(), NullLogger<RefreshTokenCommandHandler>.Instance);

    [Fact]
    public async Task Login_IssuesTokensCarryingHighestRoleAndDomainId_AndStoresOnlyAHash()
    {
        var user = await SeedLinkedUserAsync(roles: [Roles.User, Roles.Admin]);

        var result = await LoginHandler().Handle(new LoginCommand(_email, Password, "10.0.0.1"), default);

        Assert.True(result.IsSuccess);
        await _jwt.Received(1).GenerateJwtTokenAsync(
            _identityUserId, user.Id, "Student", _email, Arg.Is<IEnumerable<string>>(r => r.Single() == Roles.Admin));
        Assert.False(await _context.RefreshTokens.AnyAsync(t => t.TokenHash == result.Value.RefreshToken));
        Assert.True(await _context.RefreshTokens.AnyAsync(t => t.UserId == _identityUserId));
    }

    [Fact]
    public async Task Login_UnknownEmail_AndWrongPassword_ReturnTheSameError()
    {
        await SeedLinkedUserAsync();

        var unknown = await LoginHandler().Handle(new LoginCommand("nobody@test.local", Password, "ip"), default);
        var wrongPassword = await LoginHandler().Handle(new LoginCommand(_email, "wrong", "ip"), default);

        Assert.Equal(AuthErrors.InvalidCredentials, unknown.TopError);
        Assert.Equal(AuthErrors.InvalidCredentials, wrongPassword.TopError);
    }

    [Fact]
    public async Task Login_UnverifiedAccount_DoesNotRevealVerificationStatusWithoutThePassword()
    {
        await SeedLinkedUserAsync(verified: false);

        var wrongPassword = await LoginHandler(requireVerification: true).Handle(new LoginCommand(_email, "wrong", "ip"), default);
        var rightPassword = await LoginHandler(requireVerification: true).Handle(new LoginCommand(_email, Password, "ip"), default);

        Assert.Equal(AuthErrors.InvalidCredentials, wrongPassword.TopError);
        Assert.Equal(AuthErrors.EmailNotVerified, rightPassword.TopError);
    }

    [Fact]
    public async Task Login_BlockedAccount_GetsNoTokens()
    {
        await SeedLinkedUserAsync(active: false);

        var result = await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default);

        Assert.Equal(AuthErrors.AccountBlocked, result.TopError);
        Assert.False(await _context.RefreshTokens.AnyAsync(t => t.UserId == _identityUserId));
    }

    [Fact]
    public async Task Refresh_RotatesTheToken_AndReuseRevokesTheFamily()
    {
        await SeedLinkedUserAsync();
        var first = (await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default)).Value;

        var second = await RefreshHandler().Handle(new RefreshTokenCommand(first.RefreshToken, "ip"), default);
        var reuse = await RefreshHandler().Handle(new RefreshTokenCommand(first.RefreshToken, "ip"), default);
        var afterReuse = await RefreshHandler().Handle(new RefreshTokenCommand(second.Value.RefreshToken, "ip"), default);

        Assert.True(second.IsSuccess);
        Assert.Equal(AuthErrors.InvalidRefreshToken, reuse.TopError);
        Assert.Equal(AuthErrors.InvalidRefreshToken, afterReuse.TopError);
    }

    [Fact]
    public async Task Refresh_ForABlockedAccount_ConsumesTheTokenAndIssuesNothing()
    {
        var user = await SeedLinkedUserAsync();
        var tokens = (await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default)).Value;
        user.BlockAccount();
        await _context.SaveChangesAsync();

        var result = await RefreshHandler().Handle(new RefreshTokenCommand(tokens.RefreshToken, "ip"), default);

        Assert.Equal(AuthErrors.AccountBlocked, result.TopError);
        Assert.Null(await _refreshTokens.RedeemAsync(tokens.RefreshToken, "ip"));
    }

    [Fact]
    public async Task Logout_WithTheRefreshToken_WorksWithoutAnAccessToken()
    {
        await SeedLinkedUserAsync();
        var tokens = (await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default)).Value;
        var anonymous = new LogoutCommandHandler(TestCurrentUser.Anonymous(), _refreshTokens);

        var result = await anonymous.Handle(new LogoutCommand(tokens.RefreshToken, "ip"), default);

        Assert.True(result.IsSuccess);
        Assert.Null(await _refreshTokens.RedeemAsync(tokens.RefreshToken, "ip"));
    }

    [Fact]
    public async Task LogoutEverywhere_RequiresASignedInCaller_AndRevokesAllTheirTokens()
    {
        await SeedLinkedUserAsync();
        await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default);
        await LoginHandler().Handle(new LoginCommand(_email, Password, "ip"), default);

        var anonymous = await new LogoutCommandHandler(TestCurrentUser.Anonymous(), _refreshTokens).Handle(new LogoutCommand(null, "ip"), default);
        var stillValid = await _context.RefreshTokens.CountAsync(t => t.UserId == _identityUserId && !t.IsRevoked);
        await new LogoutCommandHandler(TestCurrentUser.InRole(Roles.User, identityUserId: _identityUserId), _refreshTokens)
            .Handle(new LogoutCommand(null, "ip"), default);

        Assert.Equal("Auth.InvalidUser", anonymous.TopError.Code);
        Assert.Equal(2, stillValid);
        Assert.False(await _context.RefreshTokens.AnyAsync(t => t.UserId == _identityUserId && !t.IsRevoked));
    }
}
