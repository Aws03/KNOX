using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.AddCourseResource;

public sealed class AddCourseResourceCommandHandler
    (IApplicationDbContext context,
     ILogger<AddCourseResourceCommandHandler> logger)
    : IRequestHandler<AddCourseResourceCommand, Result<CourseResourceDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<AddCourseResourceCommandHandler> _logger = logger;

    public async Task<Result<CourseResourceDto>> Handle(
        AddCourseResourceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adding resource to course {CourseId}", request.CourseId);

        // Get the course with CourseInfo
        var course = await _context.Courses
            .Include(c => c.CourseInfo)
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound(
                "Course.NotFound",
                $"Course with ID {request.CourseId} not found");
        }

        // Check if CourseInfo exists
        if (course.CourseInfo is null)
        {
            return Error.NotFound(
                "CourseInfo.NotFound",
                $"CourseInfo not found for course ID {request.CourseId}. Create CourseInfo first.");
        }

        // Add resource
        var resourceResult = course.CourseInfo.AddResource(
            request.Title,
            request.Type,
            request.Url,
            request.Description,
            request.DemonstrationVideoUrl);

        if (resourceResult.IsError)
            return resourceResult.Errors;

        // Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource added successfully to course {CourseId}", request.CourseId);

        return resourceResult.Value.ToDto();
    }
}
