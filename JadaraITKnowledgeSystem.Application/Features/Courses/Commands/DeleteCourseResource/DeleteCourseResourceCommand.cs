using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseResource;

public sealed record DeleteCourseResourceCommand(
    int CourseId,
    int ResourceId
) : IRequest<Result<Success>>;
