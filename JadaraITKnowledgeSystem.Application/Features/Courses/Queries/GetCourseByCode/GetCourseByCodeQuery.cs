using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseByCode;

public sealed record GetCourseByCodeQuery(string CourseCode) : IRequest<Result<CourseDto>>;
