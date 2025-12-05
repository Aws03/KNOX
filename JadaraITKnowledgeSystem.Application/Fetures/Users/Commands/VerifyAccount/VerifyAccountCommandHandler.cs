using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.VerifyAccount;

public sealed class VerifyAccountCommandHandler : IRequestHandler<VerifyAccountCommand, Result<Success>>
{
    private readonly IOTPService _otpService;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<VerifyAccountCommandHandler> _logger;

    public VerifyAccountCommandHandler(
        IOTPService otpService,
        IApplicationDbContext context,
        ILogger<VerifyAccountCommandHandler> logger)
    {
        _otpService = otpService;
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Success>> Handle(VerifyAccountCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[VerifyAccountCommand] Verifying account for {Email}", request.Email);

        // Step 1: Validate OTP
        var otpResult = await _otpService.ValidateOtpAsync(request.Email, request.Otp);
        if (otpResult.IsError)
        {
            _logger.LogWarning("[VerifyAccountCommand] OTP validation failed for {Email}: {Errors}",
                request.Email, string.Join("; ", otpResult.Errors.Select(e => e.Description)));
            return otpResult.Errors;
        }

        // Step 2: Get and verify user account
        var user = await _context.Users.FindAsync(otpResult.Value);
        if (user == null)
        {
            _logger.LogWarning("[VerifyAccountCommand] User not found with ID {UserId}", otpResult.Value);
            return Error.NotFound("User.NotFound", "User not found");
        }

        // Step 3: Mark account as verified (domain logic)
        user.VerifyAccount();
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("[VerifyAccountCommand] Account verified successfully for {Email}", request.Email);
        return Result.Success;
    }
}
