using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Auth.Commands.SendVerificationOtp;

public sealed class SendVerificationOtpCommandHandler(IOTPService otpService)
    : IRequestHandler<SendVerificationOtpCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(SendVerificationOtpCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return Error.Validation("Email", "Email is required.");

        // Unknown email and "sent too recently" share one answer to prevent account enumeration.
        return await otpService.SendOtpAsync(request.Email, cancellationToken)
            ? Result.Success
            : Error.Conflict("OTP.NotSent", "Please wait before requesting another code, or check the email address.");
    }
}
