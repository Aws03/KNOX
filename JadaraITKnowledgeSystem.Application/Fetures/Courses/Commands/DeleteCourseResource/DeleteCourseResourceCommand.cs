using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.DeleteCourseResource;

public sealed record DeleteCourseResourceCommand(
    int CourseId,
    int ResourceId
) : IRequest<Result<Success>>;
