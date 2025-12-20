using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetEnrolledCourses;

/// <summary>
/// Query to get courses the current user is enrolled in.
/// </summary>
public sealed record GetEnrolledCoursesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsFinished = null  // null = all, true = finished only, false = not finished only
) : IRequest<Result<PaginatedList<EnrolledCourseSummaryDto>>>;
