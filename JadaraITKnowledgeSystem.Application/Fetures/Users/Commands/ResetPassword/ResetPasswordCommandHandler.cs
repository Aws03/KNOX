using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Success>>
{
    private readonly IOTPService _otpService;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityUserService _identityUserService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IOTPService otpService,
        IApplicationDbContext context,
        IIdentityUserService identityUserService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _otpService = otpService;
        _context = context;
        _identityUserService = identityUserService;
        _logger = logger;
    }

    public async Task<Result<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("[ResetPasswordCommand] Resetting password for {Email}", request.Email);

        // Step 1: Validate OTP
        var otpResult = await _otpService.ValidateOtpAsync(request.Email, request.Otp);
        if (otpResult.IsError)
        {
            _logger.LogWarning("[ResetPasswordCommand] OTP validation failed for {Email}: {Errors}",
                request.Email, string.Join("; ", otpResult.Errors.Select(e => e.Description)));
            return otpResult.Errors;
        }

        // Step 2: Verify domain user exists
        var domainUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, cancellationToken);
        
        if (domainUser == null)
        {
            _logger.LogWarning("[ResetPasswordCommand] Domain user not found: {Email}", request.Email);
            return Error.NotFound("User.NotFound", "User not found");
        }

        // Step 3: Reset password in Identity system
        var resetResult = await _identityUserService.ResetPasswordAsync(request.Email, request.NewPassword);
        
        if (resetResult.IsError)
        {
            _logger.LogWarning("[ResetPasswordCommand] Password reset failed for {Email}: {Errors}",
                request.Email, string.Join("; ", resetResult.Errors.Select(e => e.Description)));
            return resetResult.Errors;
        }

        _logger.LogInformation("[ResetPasswordCommand] Password reset successfully for {Email}", request.Email);
        return Result.Success;
    }
}
