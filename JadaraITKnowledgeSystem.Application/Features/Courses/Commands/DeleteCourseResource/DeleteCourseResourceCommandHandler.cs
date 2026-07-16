using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseResource;

public sealed class DeleteCourseResourceCommandHandler
    (IApplicationDbContext context,
     ILogger<DeleteCourseResourceCommandHandler> logger)
    : IRequestHandler<DeleteCourseResourceCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<DeleteCourseResourceCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(
        DeleteCourseResourceCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting resource {ResourceId} from course {CourseId}", request.ResourceId, request.CourseId);

        // Get the course with CourseInfo and its resources - RemoveResource below
        // searches CourseInfo's in-memory Resources collection, which EF only
        // populates when explicitly included via ThenInclude.
        var course = await _context.Courses
            .Include(c => c.CourseInfo)
                .ThenInclude(ci => ci.Resources)
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

        // Remove resource
        var removeResult = course.CourseInfo.RemoveResource(request.ResourceId);
        if (removeResult.IsError)
            return removeResult.Errors;

        // Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Resource {ResourceId} deleted successfully", request.ResourceId);

        return Result.Success;
    }
}
