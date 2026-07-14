using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetEnrolledCourses;

public sealed class GetEnrolledCoursesQueryHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<GetEnrolledCoursesQueryHandler> logger)
    : IRequestHandler<GetEnrolledCoursesQuery, Result<PaginatedList<EnrolledCourseSummaryDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<GetEnrolledCoursesQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<EnrolledCourseSummaryDto>>> Handle(
        GetEnrolledCoursesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetEnrolledCourses: Page={Page}, PageSize={PageSize}, IsFinished={IsFinished}",
            request.PageNumber, request.PageSize, request.IsFinished);

        // Get current user
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value <= 0)
        {
            _logger.LogWarning("GetEnrolledCourses failed: User not authenticated");
            return Error.Unauthorized("User.NotAuthenticated", "User must be authenticated to view enrolled courses.");
        }

        var query = _context.Enrollments
            .AsNoTracking()
            .Where(e => e.UserId == userId.Value);

        // Apply filter
        if (request.IsFinished.HasValue)
        {
            query = query.Where(e => e.IsFinished == request.IsFinished.Value);
        }

        // Project to DTO with counts
        var summaryQuery = query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EnrolledCourseSummaryDto(
                EnrollmentId: e.Id,
                CourseId: e.CourseId,
                CourseName: e.Course.CourseName,
                CourseCode: e.Course.CourseCode,
                Description: e.Course.Description,
                Credits: e.Course.Credits,
                IsFinished: e.IsFinished,
                FinishedAt: e.FinishedAt,
                // TODO: Grade functionality is temporarily disabled.
                // Grade: e.Grade,
                EnrolledAt: e.CreatedAt,
                NumberOfMaterials: _context.CourseMaterials.Count(m => m.CourseId == e.CourseId),
                NumberOfQuizzes: _context.Quizzes.Count(q => q.CourseId == e.CourseId)
            ));

        var paginated = await PaginatedList<EnrolledCourseSummaryDto>.CreateAsync(
            summaryQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "GetEnrolledCourses completed: Retrieved {Count} enrollments for UserId={UserId}",
            paginated.Items.Count, userId.Value);

        return paginated;
    }
}
