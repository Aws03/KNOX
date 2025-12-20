using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.UpdateCourseInfo;

public sealed class UpdateCourseInfoCommandHandler
    (IApplicationDbContext context,
     ILogger<UpdateCourseInfoCommandHandler> logger)
    : IRequestHandler<UpdateCourseInfoCommand, Result<CourseInfoDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<UpdateCourseInfoCommandHandler> _logger = logger;

    public async Task<Result<CourseInfoDto>> Handle(
        UpdateCourseInfoCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating CourseInfo for CourseId = {CourseId}", request.CourseId);

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

        // Update course info
        var updateResult = course.UpdateCourseInfo(
            request.DifficultyLevel,
            request.Description,
            request.DemonstrationVideoUrl,
            request.DemonstrationVideoTitle);

        if (updateResult.IsError)
            return updateResult.Errors;

        // Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CourseInfo updated successfully for CourseId = {CourseId}", request.CourseId);

        return course.CourseInfo!.ToDto();
    }
}
