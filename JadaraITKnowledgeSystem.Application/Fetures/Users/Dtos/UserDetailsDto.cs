using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;

public sealed record UserDetailsDto
{
    public int Id { get; init; }
    public required FullName Name { get; init; }
    public required Email Email { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public bool IsActive { get; init; }
    public bool IsVerfied { get; init; }

    public int MajorId { get; init; }
    public string? MajorName { get; init; }

    public int? FacultyId { get; init; }
    public string? FacultyName { get; init; }

    public int? UniversityId { get; init; }
    public string? UniversityName { get; init; }
}
