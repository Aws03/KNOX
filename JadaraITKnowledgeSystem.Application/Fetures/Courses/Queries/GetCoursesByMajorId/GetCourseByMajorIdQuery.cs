using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCoursesByMajorId;

public record GetCoursesByMajorIdQuery(
    int MajorId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<CourseDto>>>;
