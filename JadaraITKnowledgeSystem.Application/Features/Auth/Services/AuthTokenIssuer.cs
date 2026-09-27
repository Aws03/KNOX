using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Auth.Errors;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Services;

/// <summary>
/// The single place access/refresh token pairs are minted, so every sign-in path
/// (login, refresh, registration, account verification) applies the same rules:
/// blocked accounts get nothing, and the JWT carries only the user's highest role.
/// </summary>
public sealed class AuthTokenIssuer(
    IApplicationDbContext context,
    IIdentityUserService identityUserService,
    IJwtTokenService jwtTokenService,
    IRefreshTokenService refreshTokenService)
{
    public async Task<Result<AuthTokensDto>> IssueAsync(
        IdentityUserInfo user,
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var isActive = await context.Users
            .Where(u => u.Id == user.DomainUserId)
            .Select(u => (bool?)u.IsActive)
            .FirstOrDefaultAsync(cancellationToken);

        if (isActive is null)
            return AuthErrors.UserNotFound;

        if (isActive == false)
            return AuthErrors.AccountBlocked;

        var roles = await identityUserService.GetRolesAsync(user.Id);
        var accessToken = await jwtTokenService.GenerateJwtTokenAsync(
            user.Id,
            user.DomainUserId,
            user.FullName,
            user.Email,
            [Roles.Highest(roles)]);

        var refreshToken = await refreshTokenService.IssueAsync(user.Id, ipAddress, cancellationToken);
        return new AuthTokensDto(accessToken, refreshToken.Token, refreshToken.ExpiresAt);
    }
}
