using JadaraITKnowledgeSystem.Domain.Users.Enums;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;

public sealed record UserDto
{
    public int Id { get; init; }
    public required FullName Name { get; init; } 
    public required Email Email { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public int MajorId { get; init; }
    }
