namespace JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;

/// <summary>
/// DTO representing a user's enrollment in a course.
/// </summary>
public sealed record EnrollmentDto(
    int Id,
    int UserId,
    int CourseId,
    string CourseName,
    string? CourseCode,
    bool IsFinished,
    DateTimeOffset? FinishedAt,
    string? Notes,
    DateTimeOffset EnrolledAt
);

/// <summary>
/// Summary DTO for enrolled courses list.
/// </summary>
public sealed record EnrolledCourseSummaryDto(
    int EnrollmentId,
    int CourseId,
    string CourseName,
    string? CourseCode,
    string? Description,
    int? Credits,
    bool IsFinished,
    DateTimeOffset? FinishedAt,
    DateTimeOffset EnrolledAt,
    int NumberOfMaterials,
    int NumberOfQuizzes
);
