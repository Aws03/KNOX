using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourse;

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

        token.ThrowIfCancellationRequested();

        // --------------------------------------------------------------------
        // 1. Resolve UniversityId from Major
        // --------------------------------------------------------------------
        var universityId = await _context.Majors
            .Where(m => m.Id == cmd.MajorId)
            .Select(m => m.Faculty.University.Id)
            .FirstOrDefaultAsync(token);

        if (universityId == 0)
        {
            _logger.LogWarning("Major not found: MajorId={MajorId}", cmd.MajorId);
            return Error.NotFound("Major.NotFound", "Major does not exist");
        }

        // --------------------------------------------------------------------
        // 2. Check if course already assigned to this major
        // --------------------------------------------------------------------
        var alreadyAssigned = await _context.MajorCourses
            .AnyAsync(mc =>
                mc.MajorId == cmd.MajorId &&
                (mc.Course.CourseCode == cmd.CourseCode ||
                 mc.Course.CourseName == cmd.CourseName),
                token);

        if (alreadyAssigned)
        {
            _logger.LogWarning(
                "Course already assigned to major: MajorId={MajorId}, Code={CourseCode}",
                cmd.MajorId, cmd.CourseCode);

            return Error.Conflict(
                "Course.Exists",
                "Course already assigned to this major");
        }

        // --------------------------------------------------------------------
        // 3. Find existing course in same university
        // --------------------------------------------------------------------
        var course = await _context.Courses
            .FirstOrDefaultAsync(c =>
                c.CourseCode == cmd.CourseCode &&
                c.Requirements.Any(r =>
                    r.Major.Faculty.University.Id == universityId),
                token);

        // --------------------------------------------------------------------
        // 4. Create & SAVE course if not exists (IMPORTANT)
        // --------------------------------------------------------------------
        if (course is null)
        {
            _logger.LogInformation(
                "Course does not exist. Creating new course: Code={CourseCode}",
                cmd.CourseCode);

            var createResult = Course.Create(
                cmd.CourseName,
                cmd.Credits,
                cmd.Description,
                cmd.CourseCode);

            if (createResult.IsError)
            {
                _logger.LogWarning(
                    "Course creation failed: {@Errors}",
                    createResult.Errors);

                return createResult.Errors;
            }

            course = createResult.Value;

            await _context.Courses.AddAsync(course, token);

            // 🔑 CRITICAL: save first to generate Course.Id
            await _context.SaveChangesAsync(token);

            _logger.LogInformation(
                "New course saved: CourseId={CourseId}",
                course.Id);
        }

        // --------------------------------------------------------------------
        // 5. Assign course to major (now Course.Id > 0)
        // --------------------------------------------------------------------
        var mappingResult = course.AssignToMajor(
            cmd.MajorId,
            cmd.RequirementType,
            cmd.RequirementNature);

        if (mappingResult.IsError)
        {
            _logger.LogWarning(
                "AssignToMajor failed: {@Errors}",
                mappingResult.Errors);

            return mappingResult.Errors;
        }

        await _context.MajorCourses.AddAsync(mappingResult.Value, token);

        // --------------------------------------------------------------------
        // 6. Save mapping
        // --------------------------------------------------------------------
        await _context.SaveChangesAsync(token);

        _logger.LogInformation(
            "Course assigned successfully: CourseId={CourseId}, MajorId={MajorId}",
            course.Id, cmd.MajorId);

        // --------------------------------------------------------------------
        // 7. Return DTO
        // --------------------------------------------------------------------
        var dto = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == course.Id)
            .Select(c => c.ToDto())
            .FirstAsync(token);

        return dto;
    }
}
