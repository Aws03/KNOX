using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Auth.Errors;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IRefreshTokenService refreshTokenService,
    IIdentityUserService identityUserService,
    AuthTokenIssuer tokenIssuer,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokensDto>>
{
    public async Task<Result<AuthTokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Rotation: the presented token is consumed whether or not a new pair is issued.
        var userId = await refreshTokenService.RedeemAsync(request.RefreshToken, request.IpAddress, cancellationToken);
        if (userId is null)
        {
            logger.LogWarning("[RefreshToken] Invalid, expired or reused refresh token");
            return AuthErrors.InvalidRefreshToken;
        }

        var user = await identityUserService.FindByIdAsync(userId.Value);
        if (user is null)
            return AuthErrors.UserNotFound;

        return await tokenIssuer.IssueAsync(user, request.IpAddress, cancellationToken);
    }
}
