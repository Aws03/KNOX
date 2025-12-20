using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseInfo;

public sealed class CreateCourseInfoCommandHandler
    (IApplicationDbContext context,
     ILogger<CreateCourseInfoCommandHandler> logger)
    : IRequestHandler<CreateCourseInfoCommand, Result<CourseInfoDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<CreateCourseInfoCommandHandler> _logger = logger;

    public async Task<Result<CourseInfoDto>> Handle(
        CreateCourseInfoCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating CourseInfo for CourseId = {CourseId}", request.CourseId);

        // Get the course
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course is null)
        {
            return Error.NotFound(
                "Course.NotFound",
                $"Course with ID {request.CourseId} not found");
        }

        // Create course info
        var courseInfoResult = course.SetCourseInfo(
            request.DifficultyLevel,
            request.Description,
            request.DemonstrationVideoUrl,
            request.DemonstrationVideoTitle);

        if (courseInfoResult.IsError)
            return courseInfoResult.Errors;

        // Persist changes
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CourseInfo created successfully for CourseId = {CourseId}", request.CourseId);

        return courseInfoResult.Value.ToDto();
    }
}
