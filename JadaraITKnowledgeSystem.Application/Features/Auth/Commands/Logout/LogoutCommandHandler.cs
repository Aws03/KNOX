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
        if (currentUser.UserId is not int userId)
            return Error.Unauthorized("Auth.InvalidUser", "Invalid user");

        // A token that isn't the caller's is silently ignored, so logout never reveals whether a token exists.
        if (string.IsNullOrEmpty(request.RefreshToken))
            await refreshTokenService.RevokeAllAsync(userId, request.IpAddress, cancellationToken);
        else
            await refreshTokenService.RevokeAsync(request.RefreshToken, userId, request.IpAddress, cancellationToken);

        return Result.Success;
    }
}
