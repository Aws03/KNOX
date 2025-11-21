using JadaraITKnowledgeSystem.Application.DTOs;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;

public sealed record CourseDto
{
    public int Id { get; init; }
    public string CourseName { get; init; } = String.Empty;
    public string? Description { get; init; }
    public string? CourseCode { get; init; }
    public int? Credits { get; init; }

    public List<CourseRequirementMappingDto> CourseRequirementMappings { get; init; } = new();

}
