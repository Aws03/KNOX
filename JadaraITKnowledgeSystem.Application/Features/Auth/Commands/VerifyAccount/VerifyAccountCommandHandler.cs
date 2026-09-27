using JadaraITKnowledgeSystem.Application.Features.Auth.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.VerifyAccount;

public sealed class VerifyAccountCommandHandler(
    IOTPService otpService,
    IApplicationDbContext context,
    IIdentityUserService identityUserService,
    AuthTokenIssuer tokenIssuer,
    ILogger<VerifyAccountCommandHandler> logger)
    : IRequestHandler<VerifyAccountCommand, Result<AuthTokensDto>>
{
    public async Task<Result<AuthTokensDto>> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        var otpResult = await otpService.ValidateOtpAsync(request.Email, request.Otp, cancellationToken);
        if (otpResult.IsError)
        {
            logger.LogWarning("[VerifyAccount] OTP validation failed: {Errors}",
                string.Join("; ", otpResult.Errors.Select(e => e.Description)));
            return otpResult.Errors;
        }

        var user = await context.Users.FindAsync([otpResult.Value], cancellationToken);
        if (user is null)
        {
            logger.LogWarning("[VerifyAccount] User not found with ID {UserId}", otpResult.Value);
            return Error.NotFound("User.NotFound", "User not found");
        }

        user.VerifyAccount();
        await context.SaveChangesAsync(cancellationToken);

        var identityUser = await identityUserService.FindByDomainUserIdAsync(user.Id, cancellationToken);
        if (identityUser is null)
        {
            logger.LogError("[VerifyAccount] No identity account linked to domain user {UserId}", user.Id);
            return Error.Unexpected("Auth.TokenGenerationFailed", "Verification successful but token generation failed");
        }

        var tokens = await tokenIssuer.IssueAsync(identityUser, request.IpAddress, cancellationToken);
        if (tokens.IsSuccess)
            logger.LogInformation("[VerifyAccount] Account verified for UserId={UserId}", user.Id);

        return tokens;
    }
}
