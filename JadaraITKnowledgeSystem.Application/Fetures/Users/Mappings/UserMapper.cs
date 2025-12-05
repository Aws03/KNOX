using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Mappings;

public static class UserMapper
{
    public static UserDto ToDto(this User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            MajorId = user.MajorId,
            ProfilePictureUrl = user.ProfilePictureUrl
        };

        return userDto;
    }

    public static List<UserDto> ToDtos(this IEnumerable<User> users)
    {
        return users.Select(user => user.ToDto()).ToList();
    }

    public static User ToEntity(this UserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);
        return User.Create(userDto.Name, userDto.Email, userDto.MajorId).Value;
    }

    public static List<User> ToEntities(this IEnumerable<UserDto> userDtos)
    {
        return userDtos.Select(dto => dto.ToEntity()).ToList();
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
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            IsVerfied = user.IsVerfied,
            ProfilePictureUrl = user.ProfilePictureUrl,
            MajorId = user.MajorId,
            MajorName = major is null ? null : $"{major.Id}-{major.Name}",
            FacultyId = faculty?.Id,
            FacultyName = faculty is null ? null : $"{faculty.Id}-{faculty.Name}",
            UniversityId = university?.Id,
            UniversityName = university is null ? null : $"{university.Id}-{university.Name}"
        };
    }
}
