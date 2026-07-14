using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Models;

public sealed record AssignCourseToMajorRequest(
    int MajorId,
    RequirementType RequirementType,
    RequirementNature RequirementNature
);
