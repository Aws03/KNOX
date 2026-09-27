using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(ICurrentUserService currentUser, IRefreshTokenService refreshTokenService)
    : IRequestHandler<LogoutCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        // Signing out one session needs only its refresh token, so it works even after the access token
        // expired. Unknown tokens are ignored, so logout never reveals whether a token exists.
        if (!string.IsNullOrEmpty(request.RefreshToken))
        {
            await refreshTokenService.RevokeAsync(request.RefreshToken, request.IpAddress, cancellationToken);
            return Result.Success;
        }

        // Signing out everywhere needs an authenticated caller.
        if (currentUser.UserId is not int userId)
            return Error.Unauthorized("Auth.InvalidUser", "Sign in, or send the refresh token to revoke.");

        await refreshTokenService.RevokeAllAsync(userId, request.IpAddress, cancellationToken);
        return Result.Success;
    }
}
