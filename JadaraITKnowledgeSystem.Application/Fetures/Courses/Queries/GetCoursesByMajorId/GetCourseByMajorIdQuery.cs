using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCoursesByMajorId;

public record GetCoursesByMajorIdQuery(
    int MajorId,
    int PageNumber = 1,
    int PageSize = 10,
    RequirementNature? RequirementNature = null,
    RequirementType? RequirementType = null
) : IRequest<Result<PaginatedList<CourseSummaryDto>>>;
