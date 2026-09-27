namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

public sealed record CourseMaterialDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;

    /// <summary>Short-lived signed URL for reading the file; request the material again once it expires.</summary>
    public string ContentUrl { get; init; } = string.Empty;

    public string ContentType { get; init; } = string.Empty;
    public long SizeBytes { get; init; }
    public int CourseId { get; init; }
    public int? FolderId { get; init; }
    public string? Description { get; init; }
    public List<string> Tags { get; init; } = new();
}
