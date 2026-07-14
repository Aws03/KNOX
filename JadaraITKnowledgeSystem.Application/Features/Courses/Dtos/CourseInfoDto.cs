using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

public sealed record CourseInfoDto
{
    public int Id { get; init; }
    public int CourseId { get; init; }
    public DifficultyLevel DifficultyLevel { get; init; }
    public string? Description { get; init; }
    public string? DemonstrationVideoUrl { get; init; }
    public string? DemonstrationVideoTitle { get; init; }
    public List<CourseResourceDto> Resources { get; init; } = new();
    public int ResourceCount { get; init; }
}
