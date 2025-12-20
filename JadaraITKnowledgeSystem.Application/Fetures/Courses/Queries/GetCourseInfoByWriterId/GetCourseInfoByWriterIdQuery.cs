using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseInfoByWriterId;

public sealed record GetCourseInfoByWriterIdQuery(int CourseId)
    : IRequest<Result<CourseInfoDto>>;
