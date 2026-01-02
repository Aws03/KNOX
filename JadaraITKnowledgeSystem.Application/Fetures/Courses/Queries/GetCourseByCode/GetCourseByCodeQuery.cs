using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseByCode;

public sealed record GetCourseByCodeQuery(string CourseCode) : IRequest<Result<CourseDto>>;
