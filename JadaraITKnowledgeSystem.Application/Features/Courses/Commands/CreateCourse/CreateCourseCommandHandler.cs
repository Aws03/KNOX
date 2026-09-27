using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourse;

/// <summary>
/// Creates a new course and assigns it to the given major. Uniqueness of the name/code
/// within the major's university is enforced by <see cref="CreateCourseCommandValidator"/>;
/// sharing an existing course with another major goes through AssignCourseToMajor.
/// </summary>
public sealed class CreateCourseCommandHandler
    : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateCourseCommandHandler> _logger;

    public CreateCourseCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateCourseCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<CourseDto>> Handle(
        CreateCourseCommand cmd,
        CancellationToken token)
    {
        _logger.LogInformation(
            "CreateCourse started: MajorId={MajorId}, Name={CourseName}, Code={CourseCode}",
            cmd.MajorId, cmd.CourseName, cmd.CourseCode);

        var majorExists = await _context.Majors.AnyAsync(m => m.Id == cmd.MajorId, token);
        if (!majorExists)
        {
            _logger.LogWarning("Major not found: MajorId={MajorId}", cmd.MajorId);
            return Error.NotFound("Major.NotFound", "Major does not exist");
        }

        var alreadyAssigned = await _context.MajorCourses
            .AnyAsync(mc =>
                mc.MajorId == cmd.MajorId &&
                (mc.Course.CourseName == cmd.CourseName ||
                 (cmd.CourseCode != null && mc.Course.CourseCode == cmd.CourseCode)),
                token);

        if (alreadyAssigned)
        {
            _logger.LogWarning(
                "Course already assigned to major: MajorId={MajorId}, Code={CourseCode}",
                cmd.MajorId, cmd.CourseCode);

            return Error.Conflict("Course.Exists", "Course already assigned to this major");
        }

        var createResult = Course.Create(cmd.CourseName, cmd.Credits, cmd.Description, cmd.CourseCode);
        if (createResult.IsError)
        {
            _logger.LogWarning("Course creation failed: {@Errors}", createResult.Errors);
            return createResult.Errors;
        }

        var course = createResult.Value;
        await _context.Courses.AddAsync(course, token);

        // The course needs its database id before a requirement mapping can reference it.
        await _context.SaveChangesAsync(token);

        var mappingResult = course.AssignToMajor(cmd.MajorId, cmd.RequirementType, cmd.RequirementNature);
        if (mappingResult.IsError)
        {
            _logger.LogWarning("AssignToMajor failed: {@Errors}", mappingResult.Errors);
            return mappingResult.Errors;
        }

        await _context.MajorCourses.AddAsync(mappingResult.Value, token);
        await _context.SaveChangesAsync(token);

        _logger.LogInformation(
            "Course created and assigned: CourseId={CourseId}, MajorId={MajorId}",
            course.Id, cmd.MajorId);

        return course.ToDto();
    }
}
