using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;

public sealed record CourseMaterialDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ContentUrl { get; init; } = string.Empty;
    public int CourseId { get; init; }
    public int? FolderId { get; init; }
    public string? Description { get; init; }
    public List<string> Tags { get; init; } = new();
}
