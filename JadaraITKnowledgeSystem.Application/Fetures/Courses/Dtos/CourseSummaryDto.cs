namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;

using JadaraITKnowledgeSystem.Domain.Courses.Enums;

public sealed record CourseSummaryDto(
    int Id,
    string CourseName,
    string? Description,
    string? CourseCode,
    int? Credits,
    RequirementType RequirementType,
    RequirementNature RequirementNature,
    int NumberOfMaterials,
    int NumberOfQuizzes
);
