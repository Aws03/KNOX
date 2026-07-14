using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.EnrollCourse;

public sealed class EnrollCourseCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<EnrollCourseCommandHandler> logger)
    : IRequestHandler<EnrollCourseCommand, Result<EnrollmentDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<EnrollCourseCommandHandler> _logger = logger;

    public async Task<Result<EnrollmentDto>> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling EnrollCourse: CourseId={CourseId}", request.CourseId);

        // Get current user
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value <= 0)
        {
            _logger.LogWarning("EnrollCourse failed: User not authenticated");
            return Error.Unauthorized("User.NotAuthenticated", "User must be authenticated to enroll in a course.");
        }

        // Verify user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId.Value, cancellationToken);

        if (!userExists)
        {
            _logger.LogWarning("EnrollCourse failed: User not found, UserId={UserId}", userId.Value);
            return Error.NotFound("User.NotFound", "User not found.");
        }

        // Verify course exists
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == request.CourseId, cancellationToken);

        if (!courseExists)
        {
            _logger.LogWarning("EnrollCourse failed: Course not found, CourseId={CourseId}", request.CourseId);
            return Error.NotFound("Course.NotFound", "Course not found.");
        }

        // Check if already enrolled
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.CourseId == request.CourseId, cancellationToken);

        if (existingEnrollment != null)
        {
            _logger.LogWarning("EnrollCourse failed: User already enrolled, UserId={UserId}, CourseId={CourseId}",
                userId.Value, request.CourseId);
            return Error.Conflict("Enrollment.AlreadyExists", "User is already enrolled in this course.");
        }

        // Create enrollment
        var enrollmentResult = Enrollment.Create(userId.Value, request.CourseId);
        if (enrollmentResult.IsError)
        {
            _logger.LogWarning("EnrollCourse domain creation failed: {@Errors}", enrollmentResult.Errors);
            return enrollmentResult.Errors;
        }

        var enrollment = enrollmentResult.Value;

        // Set notes if provided
        if (!string.IsNullOrWhiteSpace(request.Notes))
        {
            var notesResult = enrollment.UpdateNotes(request.Notes);
            if (notesResult.IsError)
            {
                return notesResult.Errors;
            }
        }

        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("EnrollCourse completed: EnrollmentId={EnrollmentId}, UserId={UserId}, CourseId={CourseId}",
            enrollment.Id, userId.Value, request.CourseId);

        // Fetch with course info for DTO
        var enrollmentWithCourse = await _context.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .FirstAsync(e => e.Id == enrollment.Id, cancellationToken);

        return enrollmentWithCourse.ToDto();
    }
}
