using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseById;

public sealed class GetCourseByIdQueryHandler
    (IApplicationDbContext context, ILogger<GetCourseByIdQueryHandler> logger)
    : IRequestHandler<GetCourseByIdQuery, Result<CourseDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetCourseByIdQueryHandler> _logger = logger;

    public async Task<Result<CourseDto>> Handle(
        GetCourseByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching course with Id = {CourseId}", request.CourseId);

        var course = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == request.CourseId)
            .Select(c => c.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (course is null)
        {
            return Error.NotFound(
                "Course.NotFound",
                $"No course found with ID {request.CourseId}");
        }

        return course;
    }
}
