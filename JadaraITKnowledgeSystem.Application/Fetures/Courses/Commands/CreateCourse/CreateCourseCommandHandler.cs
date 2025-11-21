using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourse;

public sealed class CreateCourseCommandHandler
    (IApplicationDbContext context, ILogger<CreateCourseCommandHandler> logger)
    : IRequestHandler<CreateCourseCommand, Result<CourseDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<CreateCourseCommandHandler> _logger = logger;

    
    public async Task<Result<CourseDto>> Handle(CreateCourseCommand cmd, CancellationToken token)
    {
        var sw = Stopwatch.StartNew();
        _logger.LogInformation(
            "Handling CreateCourse: MajorId={MajorId}, CourseName={CourseName}, CourseCode={CourseCode}",
            cmd.MajorId, cmd.CourseName, cmd.CourseCode);

        
        if (token.IsCancellationRequested)
        {
            _logger.LogWarning("CreateCourse canceled before start: MajorId={MajorId}", cmd.MajorId);
            token.ThrowIfCancellationRequested();
        }

        // 1️ Get university id via Major
        var universityId = await _context.Majors
            .Where(m => m.Id == cmd.MajorId)
            .Select(m => m.Faculty.University.Id)
            .FirstOrDefaultAsync(token);

        _logger.LogDebug("Resolved UniversityId={UniversityId} for MajorId={MajorId}", universityId, cmd.MajorId);

        if (universityId == 0)
        {
            _logger.LogWarning("Major not found: MajorId={MajorId}", cmd.MajorId);
            return Error.NotFound("Major.NotFound", "Major does not exist");
        }

        // 2️ Check if mapping already exists (course + major)
        var existingMapping = await _context.MajorCourses
            .Include(mc => mc.Course)
            .AnyAsync(mc => mc.MajorId == cmd.MajorId &&
                            (mc.Course.CourseName == cmd.CourseName || mc.Course.CourseCode == cmd.CourseCode),
                        token);

        _logger.LogDebug("Existing mapping check result: Exists={Exists}", existingMapping);

        if (existingMapping)
        {
            _logger.LogWarning(
                "Course already assigned to this major: MajorId={MajorId}, CourseName={CourseName}, CourseCode={CourseCode}",
                cmd.MajorId, cmd.CourseName, cmd.CourseCode);
            return Error.Conflict("Course.Exists", "Course already assigned to this major");
        }

        // 3️ Check if course exists globally
        var existingCourse = await _context.Courses
            .FirstOrDefaultAsync(c => c.CourseName == cmd.CourseName &&
                                        c.CourseCode == cmd.CourseCode, token);

        int courseId;
        if (existingCourse is null)
        {
            _logger.LogInformation("Course not found globally. Creating new Course entity: Name={CourseName}, Code={CourseCode}", cmd.CourseName, cmd.CourseCode);
            var courseResult = Course.Create(cmd.CourseName, cmd.Credits, cmd.Description, cmd.CourseCode);
            if (courseResult.IsError)
            {
                _logger.LogWarning("CreateCourse domain creation failed with errors: {@Errors}", courseResult.Errors);
                return courseResult.Errors;
            }

            await _context.Courses.AddAsync(courseResult.Value, token);
            await _context.SaveChangesAsync(token);
            courseId = courseResult.Value.Id;
            _logger.LogInformation("New Course persisted: CourseId={CourseId}", courseId);
        }
        else
        {
            courseId = existingCourse.Id;
            _logger.LogInformation("Using existing Course: CourseId={CourseId}", courseId);
        }

        // 4️ Create Major <--> Course mapping
        _logger.LogDebug(
            "Creating CourseRequirementMapping: CourseId={CourseId}, MajorId={MajorId}, RequirementType={RequirementType}, RequirementNature={RequirementNature}",
            courseId, cmd.MajorId, cmd.RequirementType, cmd.RequirementNature);

        var mappingResult = CourseRequirementMapping.Create(courseId, cmd.MajorId, cmd.RequirementType, cmd.RequirementNature);
        if (mappingResult.IsError)
        {
            _logger.LogWarning("Failed to create CourseRequirementMapping with errors: {@Errors}", mappingResult.Errors);
            return mappingResult.Errors;
        }

        await _context.MajorCourses.AddAsync(mappingResult.Value, token);
        await _context.SaveChangesAsync(token);
        _logger.LogInformation("CourseRequirementMapping persisted: MappingId={MappingId}", mappingResult.Value.Id);

        // 5️ Return DTO
        var dto = await _context.Courses
            .AsNoTracking()
            .Where(c => c.Id == courseId)
            .Select(c => c.ToDto())
            .FirstAsync(token);

        sw.Stop();
        _logger.LogInformation(
            "CreateCourse completed successfully in {ElapsedMs} ms: CourseId={CourseId}, MajorId={MajorId}",
            sw.ElapsedMilliseconds, courseId, cmd.MajorId);

        return dto;
        

    }
}
