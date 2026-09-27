namespace JadaraITKnowledgeSystem.Application.Features.Users.Dtos;

public sealed record UserDetailsDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    /// <summary>Lower-cased for display (addresses are stored normalized).</summary>
    public required string Email { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public bool IsActive { get; init; }
    public bool IsVerified { get; init; }

    public int MajorId { get; init; }
    public string? MajorName { get; init; }

    public int? FacultyId { get; init; }
    public string? FacultyName { get; init; }

    public int? UniversityId { get; init; }
    public string? UniversityName { get; init; }
}
