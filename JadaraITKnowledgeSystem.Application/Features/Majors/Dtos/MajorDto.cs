using JadaraITKnowledgeSystem.Application.DTOs;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;

public sealed record MajorDto
{
    public int Id { get; init; }
    public string Name { get; init; } = String.Empty;
    public int FacultyId { get; init; }
    public List<CourseRequirementMappingDto> CourseRequirementMappings { get; init; } = new();
}
