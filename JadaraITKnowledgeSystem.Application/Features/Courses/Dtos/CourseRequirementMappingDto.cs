using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

public sealed record CourseRequirementMappingDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int MajorId { get; set; }
    public RequirementType RequirementType { get; set; } // (University, Faculty, Major, Remedial)
    public RequirementNature RequirementNature { get; set; } // (Compulsory, Elective)
}
