using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.EnrollCourse;

/// <summary>
/// Command to enroll a user in a course.
/// </summary>
public sealed record EnrollCourseCommand(
    int CourseId,
    string? Notes = null
) : IRequest<Result<EnrollmentDto>>;
