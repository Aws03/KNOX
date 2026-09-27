using JadaraITKnowledgeSystem.Application.Common.Options;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    ISender sender,
    IOTPService otpService,
    IIdentityUserService identityUserService,
    AuthTokenIssuer tokenIssuer,
    IOptions<AuthOptions> authOptions,
    ILogger<RegisterCommandHandler> logger)
    : IRequestHandler<RegisterCommand, Result<RegistrationResultDto>>
{
    public async Task<Result<RegistrationResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var created = await sender.Send(
            new CreateUserCommand(request.FullName, request.Email, request.MajorId, request.Password),
            cancellationToken);

        if (created.IsError)
            return created.Errors;

        var user = created.Value;

        if (authOptions.Value.RequireEmailVerification)
        {
            var otpSent = await TrySendOtpAsync(request.Email, cancellationToken);
            return new RegistrationResultDto(user, RequiresVerification: true, otpSent, Tokens: null);
        }

        var identityUser = await identityUserService.FindByIdAsync(user.IdentityUserId);
        if (identityUser is null)
        {
            logger.LogError("[Register] Identity user {IdentityUserId} not found right after creation", user.IdentityUserId);
            return Error.Unexpected("Auth.TokenGenerationFailed", "User created but token generation failed");
        }

        var tokens = await tokenIssuer.IssueAsync(identityUser, request.IpAddress, cancellationToken);
        if (tokens.IsError)
            return Error.Unexpected("Auth.TokenGenerationFailed", "Token generation failed");

        return new RegistrationResultDto(user, RequiresVerification: false, OtpSent: false, tokens.Value);
    }

    // The account already exists at this point; an email-provider outage must not
    // fail the registration - the user can request a new code from the verify page.
    private async Task<bool> TrySendOtpAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            return await otpService.SendOtpAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[Register] Failed to send verification OTP");
            return false;
        }
    }
}
