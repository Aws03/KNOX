using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCoursesByMajorId;

public class GetCoursesByMajorIdQueryHandler
    (IApplicationDbContext context, ILogger<GetCoursesByMajorIdQueryHandler> logger)
    : IRequestHandler<GetCoursesByMajorIdQuery, Result<PaginatedList<CourseSummaryDto>>>
{
    private readonly ILogger<GetCoursesByMajorIdQueryHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PaginatedList<CourseSummaryDto>>> Handle(
        GetCoursesByMajorIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Retrieving course summaries for MajorId {MajorId}, Page {Page}, PageSize {PageSize}, RequirementNature={RequirementNature}, RequirementType={RequirementType}",
            request.MajorId, request.PageNumber, request.PageSize, request.RequirementNature, request.RequirementType);

        var mappingQuery = _context.MajorCourses
            .AsNoTracking()
            .Where(mc => mc.MajorId == request.MajorId);

        if (request.RequirementNature.HasValue)
        {
            mappingQuery = mappingQuery.Where(mc => mc.RequirementNature == request.RequirementNature.Value);
        }

        if (request.RequirementType.HasValue)
        {
            mappingQuery = mappingQuery.Where(mc => mc.RequirementType == request.RequirementType.Value);
        }

        // Project directly to summary dto. Counts done via subqueries.
        var summaryQuery = mappingQuery
            .Select(mc => new CourseSummaryDto(
                Id: mc.Course.Id,
                CourseName: mc.Course.CourseName,
                Description: mc.Course.Description,
                CourseCode: mc.Course.CourseCode,
                Credits: mc.Course.Credits,
                RequirementType: mc.RequirementType,
                RequirementNature: mc.RequirementNature,
                NumberOfMaterials: _context.CourseMaterials.Count(m => m.CourseId == mc.CourseId),
                NumberOfQuizzes: _context.Quizzes.Count(q => q.CourseId == mc.CourseId),
                HasCourseInfo: mc.Course.CourseInfo != null
            ));

        var paginated = await PaginatedList<CourseSummaryDto>.CreateAsync(
            summaryQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} course summaries for MajorId {MajorId} on Page {Page}",
            paginated.Items.Count, request.MajorId, request.PageNumber);

        return paginated;
    }
}
