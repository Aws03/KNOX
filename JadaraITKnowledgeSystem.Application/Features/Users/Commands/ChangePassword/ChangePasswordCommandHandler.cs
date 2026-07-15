using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<Success>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly IApplicationDbContext _context;
    private readonly IIdentityUserService _identityUserService;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        ICurrentUserService currentUser,
        IApplicationDbContext context,
        IIdentityUserService identityUserService,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _currentUser = currentUser;
        _context = context;
        _identityUserService = identityUserService;
        _logger = logger;
    }

    public async Task<Result<Success>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
        {
            _logger.LogWarning("[ChangePassword] Unauthorized attempt - no user ID");
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");
        }

        var userEmail = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(userEmail))
        {
            _logger.LogWarning("[ChangePassword] Unauthorized attempt - no email for UserId={UserId}", userId.Value);
            return Error.Unauthorized("User.NotAuthenticated", "User email not found.");
        }

        _logger.LogInformation("[ChangePassword] Changing password for UserId={UserId}", userId.Value);

        // Verify domain user exists
        var domainUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == userEmail, cancellationToken);
        
        if (domainUser == null)
        {
            _logger.LogWarning("[ChangePassword] Domain user not found for email {Email}", userEmail);
            return Error.NotFound("User.NotFound", "User not found");
        }

        // Change password in Identity system
        var changeResult = await _identityUserService.ChangePasswordAsync(
            userId.Value, 
            request.CurrentPassword, 
            request.NewPassword);
        
        if (changeResult.IsError)
        {
            _logger.LogWarning("[ChangePassword] Password change failed for UserId={UserId}: {Errors}",
                userId.Value, string.Join("; ", changeResult.Errors.Select(e => e.Description)));
            return changeResult.Errors;
        }

        _logger.LogInformation("[ChangePassword] Password changed successfully for UserId={UserId}", userId.Value);
        return Result.Success;
    }
}
