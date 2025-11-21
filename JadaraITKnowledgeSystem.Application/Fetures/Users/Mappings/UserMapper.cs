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
            PermissionLevel = user.PermissionLevel
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
        return User.Create(userDto.Name, userDto.Email, userDto.PermissionLevel).Value;
    }

    public static List<User> ToEntities(this IEnumerable<UserDto> userDtos)
    {
        return userDtos.Select(dto => dto.ToEntity()).ToList();
    }
}
