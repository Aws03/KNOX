using JadaraITKnowledgeSystem.Application.Common.Options;
using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Auth.Errors;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IIdentityUserService identityUserService,
    IApplicationDbContext context,
    AuthTokenIssuer tokenIssuer,
    IOptions<AuthOptions> authOptions,
    ILogger<LoginCommandHandler> logger)
    : IRequestHandler<LoginCommand, Result<AuthTokensDto>>
{
    public async Task<Result<AuthTokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrEmpty(request.Password))
            return AuthErrors.InvalidCredentials;

        var user = await identityUserService.FindByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogWarning("[Login] No account for the supplied email");
            return AuthErrors.InvalidCredentials;
        }

        // Password first: nothing about the account (verified or not) is revealed to
        // a caller who can't prove they own it.
        if (!await identityUserService.CheckPasswordAsync(user.Id, request.Password))
        {
            logger.LogWarning("[Login] Password check failed for UserId={UserId}", user.Id);
            return AuthErrors.InvalidCredentials;
        }

        if (authOptions.Value.RequireEmailVerification)
        {
            var isVerified = await context.Users
                .Where(u => u.Id == user.DomainUserId)
                .Select(u => u.IsVerified)
                .FirstOrDefaultAsync(cancellationToken);

            if (!isVerified)
            {
                logger.LogWarning("[Login] Email not verified for UserId={UserId}", user.Id);
                return AuthErrors.EmailNotVerified;
            }
        }

        var tokens = await tokenIssuer.IssueAsync(user, request.IpAddress, cancellationToken);
        if (tokens.IsSuccess)
            logger.LogInformation("[Login] Login successful for UserId={UserId}", user.Id);

        return tokens;
    }
}
