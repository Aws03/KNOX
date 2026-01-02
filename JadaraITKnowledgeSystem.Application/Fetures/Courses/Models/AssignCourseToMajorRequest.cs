using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Models;

public sealed record AssignCourseToMajorRequest(
    int MajorId,
    RequirementType RequirementType,
    RequirementNature RequirementNature
);
