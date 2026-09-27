using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

public sealed record CourseDto
{
    public int Id { get; init; }
    public string CourseName { get; init; } = String.Empty;
    public string? Description { get; init; }
    public string? CourseCode { get; init; }
    public int? Credits { get; init; }

    /// <summary>Filled when a single course is fetched (by id or code); 0 in lists and on creation.</summary>
    public int NumberOfMaterials { get; init; }

    public int NumberOfQuizzes { get; init; }

    public List<CourseRequirementMappingDto> CourseRequirementMappings { get; init; } = new();

}
