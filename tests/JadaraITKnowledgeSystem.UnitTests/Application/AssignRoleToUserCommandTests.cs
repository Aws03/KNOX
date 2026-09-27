using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.UnitTests.TestSupport;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace JadaraITKnowledgeSystem.UnitTests.Application;

public class AssignRoleToUserCommandTests
{
    private const int DomainUserId = 20;
    private const int IdentityUserId = 70;

    private readonly IIdentityUserService _identity = Substitute.For<IIdentityUserService>();

    public AssignRoleToUserCommandTests()
    {
        var target = new IdentityUserInfo(IdentityUserId, DomainUserId, "Target", "target@test.local");
        _identity.FindByDomainUserIdAsync(DomainUserId, Arg.Any<CancellationToken>()).Returns(target);
        _identity.FindByIdAsync(IdentityUserId).Returns(target);
        _identity.GetRolesAsync(IdentityUserId).Returns([Roles.User]);
        _identity.SetSingleRoleAsync(default, default!).ReturnsForAnyArgs(Result.Success);
    }

    private AssignRoleToUserCommandHandler Handler(string callerRole) =>
        new(_identity, TestCurrentUser.InRole(callerRole), NullLogger<AssignRoleToUserCommandHandler>.Instance);

    [Fact]
    public async Task AnAdministrator_CannotChangeTheirOwnRole()
    {
        var self = new AssignRoleToUserCommandHandler(
            _identity, TestCurrentUser.InRole(Roles.SuperAdmin, identityUserId: IdentityUserId), NullLogger<AssignRoleToUserCommandHandler>.Instance);

        var result = await self.Handle(new AssignRoleToUserCommand(DomainUserId, Roles.Writer), default);

        Assert.Equal("Role.CannotChangeOwn", result.TopError.Code);
        await _identity.DidNotReceiveWithAnyArgs().SetSingleRoleAsync(default, default!);
    }

    [Fact]
    public async Task DomainUserId_IsResolvedToTheLinkedIdentityAccount()
    {
        var result = await Handler(Roles.Admin).Handle(new AssignRoleToUserCommand(DomainUserId, "writer"), default);

        Assert.True(result.IsSuccess);
        // Previously the domain id was passed straight through as if it were the identity id.
        await _identity.Received(1).SetSingleRoleAsync(IdentityUserId, Roles.Writer);
    }

    [Fact]
    public async Task IdentityUserId_CanBeUsedByTheLegacyAuthEndpoint()
    {
        var result = await Handler(Roles.Admin).Handle(
            new AssignRoleToUserCommand(IdentityUserId, Roles.Writer, UserIdIsIdentityId: true), default);

        Assert.True(result.IsSuccess);
        await _identity.Received(1).SetSingleRoleAsync(IdentityUserId, Roles.Writer);
    }

    [Fact]
    public async Task Admin_CannotGrantSuperAdmin()
    {
        var result = await Handler(Roles.Admin).Handle(new AssignRoleToUserCommand(DomainUserId, Roles.SuperAdmin), default);

        Assert.Equal(ErrorKind.Forbidden, result.TopError.Type);
        await _identity.DidNotReceiveWithAnyArgs().SetSingleRoleAsync(default, default!);
    }

    [Fact]
    public async Task Admin_CannotDemoteASuperAdmin()
    {
        _identity.GetRolesAsync(IdentityUserId).Returns([Roles.SuperAdmin]);

        var result = await Handler(Roles.Admin).Handle(new AssignRoleToUserCommand(DomainUserId, Roles.User), default);

        Assert.Equal(ErrorKind.Forbidden, result.TopError.Type);
    }

    [Fact]
    public async Task SuperAdmin_CanGrantSuperAdmin()
    {
        var result = await Handler(Roles.SuperAdmin).Handle(new AssignRoleToUserCommand(DomainUserId, Roles.SuperAdmin), default);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UnknownRole_IsAValidationError()
    {
        var result = await Handler(Roles.SuperAdmin).Handle(new AssignRoleToUserCommand(DomainUserId, "Owner"), default);

        Assert.Equal(ErrorKind.Validation, result.TopError.Type);
    }

    [Fact]
    public async Task UnknownUser_IsNotFound()
    {
        var result = await Handler(Roles.SuperAdmin).Handle(new AssignRoleToUserCommand(999, Roles.Writer), default);

        Assert.Equal(ErrorKind.NotFound, result.TopError.Type);
    }

    [Fact]
    public void Roles_Highest_FollowsTheHierarchy()
    {
        Assert.Equal(Roles.SuperAdmin, Roles.Highest(["User", "superadmin", "Writer"]));
        Assert.Equal(Roles.Writer, Roles.Highest([Roles.User, Roles.Writer]));
        Assert.Equal(Roles.User, Roles.Highest([]));
    }
}
