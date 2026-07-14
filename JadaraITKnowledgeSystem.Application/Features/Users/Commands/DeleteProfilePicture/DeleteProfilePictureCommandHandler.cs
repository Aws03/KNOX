using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.DeleteProfilePicture;

public sealed class DeleteProfilePictureCommandHandler
    (IApplicationDbContext context, 
     ICurrentUserService currentUser, 
     IFileManager fileManager,
     ILogger<DeleteProfilePictureCommandHandler> logger)
    : IRequestHandler<DeleteProfilePictureCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IFileManager _fileManager = fileManager;
    private readonly ILogger<DeleteProfilePictureCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(
        DeleteProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

        var userEmail = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(userEmail))
            return Error.Unauthorized("User.NotAuthenticated", "User email not found.");

        _logger.LogInformation("Deleting profile picture for User {UserId}", userId.Value);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Address == userEmail, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found.");

        // Check if user has a profile picture
        if (string.IsNullOrWhiteSpace(user.ProfilePictureUrl))
            return Error.NotFound("ProfilePicture.NotFound", "User does not have a profile picture.");

        // Delete from external storage
        var deleted = await _fileManager.DeleteAsync(user.ProfilePictureUrl, cancellationToken);
        if (!deleted)
        {
            _logger.LogWarning("Failed to delete profile picture from storage for User {UserId}", userId.Value);
        }

        // Remove from user entity
        user.RemoveProfilePicture();
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile picture deleted successfully for User {UserId}", userId.Value);

        return Result.Success;
    }
}
