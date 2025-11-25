

namespace JadaraITKnowledgeSystem.Application.DTOs;

public sealed record CourseMaterialDto
{
    public int Id { get; init; }
    public string Title { get; init; } = String.Empty;
    public string ContentUrl { get; init; } = String.Empty;
    public int CourseId { get; init; }
    public int? FolderId { get; init; }
    public string? Description { get; init; }
}
