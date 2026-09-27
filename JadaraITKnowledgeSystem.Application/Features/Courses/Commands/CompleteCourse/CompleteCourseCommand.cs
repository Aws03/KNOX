using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CompleteCourse;

/// <summary>
/// Command to mark the current user's enrollment in a course as completed.
/// </summary>
public sealed record CompleteCourseCommand(int CourseId) : IRequest<Result<EnrollmentDto>>;
