using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler
    (IApplicationDbContext context, ICurrentUserService currentUser, ILogger<UpdateUserProfileCommandHandler> logger)
    : IRequestHandler<UpdateUserProfileCommand, Result<UserProfileDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger = logger;

    public async Task<Result<UserProfileDto>> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

        var userEmail = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(userEmail))
            return Error.Unauthorized("User.NotAuthenticated", "User email not found.");

        _logger.LogInformation("Updating profile for User {UserId}", userId.Value);

        var user = await _context.Users
            .Include(u => u.Major)
                .ThenInclude(m => m.Faculty)
                    .ThenInclude(f => f.University)
            .FirstOrDefaultAsync(u => u.Email.Address == userEmail, cancellationToken);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found.");

        // Update full name if provided
        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            try
            {
                var fullName = new FullName(request.FullName);
                user.UpdateName(fullName);
            }
            catch (ArgumentException ex)
            {
                return Error.Validation("FullName.Invalid", ex.Message);
            }
        }

        // Update major if provided
        if (request.MajorId.HasValue)
        {
            var majorExists = await _context.Majors
                .AnyAsync(m => m.Id == request.MajorId.Value, cancellationToken);

            if (!majorExists)
                return Error.NotFound("Major.NotFound", $"Major with ID {request.MajorId.Value} not found.");

            user.UpdateMajor(request.MajorId.Value);

            // Reload the major navigation property
            await _context.Users.Entry(user)
                .Reference(u => u.Major)
                .Query()
                .Include(m => m.Faculty)
                    .ThenInclude(f => f.University)
                .LoadAsync(cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile updated successfully for User {UserId}", userId.Value);

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
            IsVerified = user.IsVerified,
            VerificationDate = user.VerificationDate,
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
