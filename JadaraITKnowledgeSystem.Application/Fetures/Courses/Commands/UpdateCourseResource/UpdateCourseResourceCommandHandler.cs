using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.UpdateCourseResource;

public sealed class UpdateCourseResourceCommandHandler
    (IApplicationDbContext context,
     ILogger<UpdateCourseResourceCommandHandler> logger)
    : IRequestHandler<UpdateCourseResourceCommand, Result<CourseResourceDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<UpdateCourseResourceCommandHandler> _logger = logger;

    public async Task<Result<CourseResourceDto>> Handle(
        UpdateCourseResourceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating resource {ResourceId} for course {CourseId}", request.ResourceId, request.CourseId);

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
                $"CourseInfo not found for course ID {request.CourseId}");
        }

        // Get the resource
        var resourceResult = course.CourseInfo.GetResource(request.ResourceId);
        if (resourceResult.IsError)
            return resourceResult.Errors;

        var resource = resourceResult.Value;

        // Update resource fields if provided
        if (!string.IsNullOrWhiteSpace(request.Title))
        {
            resource.UpdateTitle(request.Title);
        }

        if (request.Type.HasValue)
        {
            resource.UpdateType(request.Type.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Url))
        {
            var urlResult = resource.UpdateUrl(request.Url);
            if (urlResult.IsError)
                return urlResult.Errors;
        }

        if (request.Description is not null)
        {
            resource.UpdateDescription(request.Description);
        }

        if (request.DemonstrationVideoUrl is not null)
        {
            var videoResult = resource.SetDemonstrationVideo(request.DemonstrationVideoUrl);
            if (videoResult.IsError)
                return videoResult.Errors;
        }

        // Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource {ResourceId} updated successfully", request.ResourceId);

        return resource.ToDto();
    }
}
