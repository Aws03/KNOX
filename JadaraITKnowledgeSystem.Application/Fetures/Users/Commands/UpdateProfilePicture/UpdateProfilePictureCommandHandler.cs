using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.UpdateProfilePicture;

public sealed class UpdateProfilePictureCommandHandler
    (IApplicationDbContext context,
     ICurrentUserService currentUser,
     IFileManager fileManager,
     ILogger<UpdateProfilePictureCommandHandler> logger)
    : IRequestHandler<UpdateProfilePictureCommand, Result<UserProfileDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly IFileManager _fileManager = fileManager;
    private readonly ILogger<UpdateProfilePictureCommandHandler> _logger = logger;

    public async Task<Result<UserProfileDto>> Handle(
        UpdateProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

        var userEmail = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(userEmail))
            return Error.Unauthorized("User.NotAuthenticated", "User email not found.");

        _logger.LogInformation("Updating profile picture for User {UserId}", userId.Value);

        var user = await _context.Users
            .Include(u => u.Major)
                .ThenInclude(m => m.Faculty)
                    .ThenInclude(f => f.University)
            .FirstOrDefaultAsync(u => u.Email.Address == userEmail, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found.");

        // Get file extension
        var extension = Path.GetExtension(request.FileName);
        if (string.IsNullOrWhiteSpace(extension))
            return Error.Validation("Image.InvalidExtension", "File must have a valid extension.");

        // Upload new profile picture (UpdateAsync handles deleting old file if exists)
        var newImageUrl = await _fileManager.UpdateAsync(
            user.ProfilePictureUrl,
            request.ImageStream,
            extension,
            $"profile-pictures/{userId}",
            cancellationToken);

        // Update user with new profile picture URL
        user.SetProfilePicture(newImageUrl);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile picture updated successfully for User {UserId}", userId.Value);

        var role = _currentUser.Roles.FirstOrDefault() ?? "User";

        var dto = new UserProfileDto
        {
            IdentityUserId = userId.Value,
            DomainUserId = user.Id,
            Email = userEmail,
            FullName = user.Name.ToString(),
            DateJoined = user.CreatedAt.UtcDateTime,
            Role = role,
            IsActive = user.IsActive,
            IsVerfied = user.IsVerfied,
            VerficationDate = user.VerficationDate,
            ProfilePictureUrl = user.ProfilePictureUrl,
            UniversityId = user.Major.Faculty.University.Id,
            UniversityName = user.Major.Faculty.University.Name,
            FacultyId = user.Major.Faculty.Id,
            FacultyName = user.Major.Faculty.Name,
            MajorId = user.Major.Id,
            MajorName = user.Major.Name
        };

        return dto;
    }
}
