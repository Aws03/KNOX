using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

public sealed record CourseResourceDto
{
    public int Id { get; init; }
    public int CourseInfoId { get; init; }
    public string Title { get; init; } = string.Empty;
    public ResourceType Type { get; init; }
    public string Url { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? DemonstrationVideoUrl { get; init; }
    public bool HasDemonstrationVideo { get; init; }
}
