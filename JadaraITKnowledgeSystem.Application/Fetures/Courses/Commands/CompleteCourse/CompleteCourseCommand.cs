using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CompleteCourse;

/// <summary>
/// Command to mark a course enrollment as completed/finished.
/// </summary>
// TODO: Grade functionality is temporarily disabled.
// Universities may have different grading systems (A, A+, B, etc.)
// Optionally includes a grade parameter.
public sealed record CompleteCourseCommand(
    int CourseId
    // decimal? Grade = null
) : IRequest<Result<EnrollmentDto>>;
