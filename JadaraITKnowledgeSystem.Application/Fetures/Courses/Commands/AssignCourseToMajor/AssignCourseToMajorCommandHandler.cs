using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.AssignCourseToMajor;

public sealed class AssignCourseToMajorCommandHandler
    (IApplicationDbContext context, ILogger<AssignCourseToMajorCommandHandler> logger)
    : IRequestHandler<AssignCourseToMajorCommand, Result<CourseRequirementMappingDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<AssignCourseToMajorCommandHandler> _logger = logger;

    public async Task<Result<CourseRequirementMappingDto>> Handle(
        AssignCourseToMajorCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Assigning Course {CourseId} to Major {MajorId} with RequirementType={RequirementType}, RequirementNature={RequirementNature}",
            command.CourseId, command.MajorId, command.RequirementType, command.RequirementNature);

        // 1. Verify the course exists
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == command.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course not found: CourseId={CourseId}", command.CourseId);
            return Error.NotFound("Course.NotFound", $"Course with ID {command.CourseId} does not exist.");
        }

        // 2. Verify the major exists
        var majorExists = await _context.Majors
            .AnyAsync(m => m.Id == command.MajorId, cancellationToken);

        if (!majorExists)
        {
            _logger.LogWarning("Major not found: MajorId={MajorId}", command.MajorId);
            return Error.NotFound("Major.NotFound", $"Major with ID {command.MajorId} does not exist.");
        }

        // 3. Check if the course is already assigned to this major
        var existingMapping = await _context.MajorCourses
            .AnyAsync(mc => mc.CourseId == command.CourseId && mc.MajorId == command.MajorId, cancellationToken);

        if (existingMapping)
        {
            _logger.LogWarning(
                "Course already assigned to Major: CourseId={CourseId}, MajorId={MajorId}",
                command.CourseId, command.MajorId);
            return Error.Conflict("Course.AlreadyAssigned", "This course is already assigned to the specified major.");
        }

        // 4. Use the domain method to assign the course to the major
        var assignResult = course.AssignToMajor(command.MajorId, command.RequirementType, command.RequirementNature);

        if (assignResult.IsError)
        {
            _logger.LogWarning(
                "Failed to assign Course to Major with errors: {@Errors}",
                assignResult.Errors);
            return assignResult.Errors;
        }

        // 5. Save changes to persist the new mapping
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Course assigned to Major successfully: CourseId={CourseId}, MajorId={MajorId}, MappingId={MappingId}",
            command.CourseId, command.MajorId, assignResult.Value.Id);

        // 6. Return the DTO
        return assignResult.Value.ToDto();
    }
}
