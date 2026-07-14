using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseInfoByCourseId;

public sealed record GetCourseInfoByCourseIdQuery(int CourseId)
    : IRequest<Result<CourseInfoDto>>;
