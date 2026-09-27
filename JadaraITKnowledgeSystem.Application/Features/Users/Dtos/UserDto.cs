namespace JadaraITKnowledgeSystem.Application.Features.Users.Dtos;

public sealed record UserDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    /// <summary>Lower-cased for display (addresses are stored normalized).</summary>
    public required string Email { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public int MajorId { get; init; }
}
