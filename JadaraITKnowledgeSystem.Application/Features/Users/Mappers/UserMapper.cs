using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name.Value,
            Email = DisplayEmail(user),
            MajorId = user.MajorId,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public static UserDetailsDto ToDetailsDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var university = user.Major?.Faculty?.University;
        var faculty = user.Major?.Faculty;
        var major = user.Major;

        return new UserDetailsDto
        {
            Id = user.Id,
            Name = user.Name.Value,
            Email = DisplayEmail(user),
            IsActive = user.IsActive,
            IsVerified = user.IsVerified,
            ProfilePictureUrl = user.ProfilePictureUrl,
            MajorId = user.MajorId,
            MajorName = major?.Name,
            FacultyId = faculty?.Id,
            FacultyName = faculty?.Name,
            UniversityId = university?.Id,
            UniversityName = university?.Name
        };
    }

    private static string DisplayEmail(User user) => user.Email.Address.ToLowerInvariant();

    /// <summary>
    /// Builds the signed-in user's profile. <paramref name="user"/> must be loaded with
    /// Major -> Faculty -> University.
    /// </summary>
    public static UserProfileDto ToProfileDto(this User user, int identityUserId, string email, string role)
    {
        ArgumentNullException.ThrowIfNull(user);

        var major = user.Major;
        var faculty = major.Faculty;
        var university = faculty.University;

        return new UserProfileDto
        {
            IdentityUserId = identityUserId,
            DomainUserId = user.Id,
            Email = email,
            FullName = user.Name.ToString(),
            DateJoined = user.CreatedAt.UtcDateTime,
            Role = role,
            IsActive = user.IsActive,
            IsVerified = user.IsVerified,
            VerificationDate = user.VerificationDate,
            ProfilePictureUrl = user.ProfilePictureUrl,
            UniversityId = university.Id,
            UniversityName = university.Name,
            FacultyId = faculty.Id,
            FacultyName = faculty.Name,
            MajorId = major.Id,
            MajorName = major.Name
        };
    }
}
