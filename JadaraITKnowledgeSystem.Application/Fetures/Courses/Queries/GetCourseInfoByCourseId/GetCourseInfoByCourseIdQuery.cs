using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseInfoByCourseId;

public sealed record GetCourseInfoByCourseIdQuery(int CourseId)
    : IRequest<Result<CourseInfoDto>>;
