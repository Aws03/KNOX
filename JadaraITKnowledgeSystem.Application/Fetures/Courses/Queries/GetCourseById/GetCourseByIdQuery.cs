using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseById;

public sealed record GetCourseByIdQuery(int CourseId)
    : IRequest<Result<CourseDto>>;
