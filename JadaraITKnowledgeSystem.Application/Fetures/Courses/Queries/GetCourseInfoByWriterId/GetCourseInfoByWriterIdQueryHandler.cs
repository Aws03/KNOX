using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseInfoByWriterId;

public sealed class GetCourseInfoByWriterIdQueryHandler
    (IApplicationDbContext context, ICurrentUserService currentUser, ILogger<GetCourseInfoByWriterIdQueryHandler> logger)
    : IRequestHandler<GetCourseInfoByWriterIdQuery, Result<CourseInfoDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUser = currentUser;
    private readonly ILogger<GetCourseInfoByWriterIdQueryHandler> _logger = logger;

    public async Task<Result<CourseInfoDto>> Handle(
        GetCourseInfoByWriterIdQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (!userId.HasValue)
            return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

        // Get user's email from Identity
        var userEmail = _currentUser.Email;
        if (string.IsNullOrWhiteSpace(userEmail))
            return Error.Unauthorized("User.NotAuthenticated", "User email not found.");

        _logger.LogInformation("Fetching CourseInfo for CourseId = {CourseId}, Writer = {Email}", request.CourseId, userEmail);

        var courseInfo = await _context.CourseInfos
            .AsNoTracking()
            .Include(ci => ci.Resources.Where(r => r.CreatedBy == userEmail))
            .FirstOrDefaultAsync(ci => ci.CourseId == request.CourseId, cancellationToken);

        if (courseInfo is null)
        {
            return Error.NotFound(
                "CourseInfo.NotFound",
                $"No course info found for course ID {request.CourseId}");
        }

        return courseInfo.ToDto();
    }
}
