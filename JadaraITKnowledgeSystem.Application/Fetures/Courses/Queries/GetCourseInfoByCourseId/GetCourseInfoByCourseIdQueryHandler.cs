using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseInfoByCourseId;

public sealed class GetCourseInfoByCourseIdQueryHandler
    (IApplicationDbContext context, ILogger<GetCourseInfoByCourseIdQueryHandler> logger)
    : IRequestHandler<GetCourseInfoByCourseIdQuery, Result<CourseInfoDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetCourseInfoByCourseIdQueryHandler> _logger = logger;

    public async Task<Result<CourseInfoDto>> Handle(
        GetCourseInfoByCourseIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching CourseInfo for CourseId = {CourseId}", request.CourseId);

        var courseInfo = await _context.CourseInfos
            .AsNoTracking()
            .Include(ci => ci.Resources)
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
