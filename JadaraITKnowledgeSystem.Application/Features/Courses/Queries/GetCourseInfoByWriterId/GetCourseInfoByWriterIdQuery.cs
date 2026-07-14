using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByWriterId;

public sealed record GetCourseInfoByWriterIdQuery(int CourseId)
    : IRequest<Result<CourseInfoDto>>;
