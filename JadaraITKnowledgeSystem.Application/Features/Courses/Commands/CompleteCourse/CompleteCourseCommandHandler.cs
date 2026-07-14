using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CompleteCourse;

public sealed class CompleteCourseCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<CompleteCourseCommandHandler> logger)
    : IRequestHandler<CompleteCourseCommand, Result<EnrollmentDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<CompleteCourseCommandHandler> _logger = logger;

    public async Task<Result<EnrollmentDto>> Handle(CompleteCourseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CompleteCourse: CourseId={CourseId}",
            request.CourseId);

        // Get current user
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value <= 0)
        {
            _logger.LogWarning("CompleteCourse failed: User not authenticated");
            return Error.Unauthorized("User.NotAuthenticated", "User must be authenticated to complete a course.");
        }

        // Find enrollment
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.CourseId == request.CourseId, cancellationToken);

        if (enrollment == null)
        {
            _logger.LogWarning("CompleteCourse failed: Enrollment not found, UserId={UserId}, CourseId={CourseId}",
                userId.Value, request.CourseId);
            return Error.NotFound("Enrollment.NotFound", "User is not enrolled in this course.");
        }

        // Complete the course
        // TODO: Grade functionality is temporarily disabled.
        // Universities may have different grading systems (A, A+, B, etc.)
        // Result<Success> result;
        // if (request.Grade.HasValue)
        // {
        //     result = enrollment.CompleteWithGrade(request.Grade.Value);
        // }
        // else
        // {
        //     result = enrollment.Complete();
        // }
        var result = enrollment.Complete();

        if (result.IsError)
        {
            _logger.LogWarning("CompleteCourse domain operation failed: {@Errors}", result.Errors);
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CompleteCourse completed: EnrollmentId={EnrollmentId}, UserId={UserId}, CourseId={CourseId}",
            enrollment.Id, userId.Value, request.CourseId);

        return enrollment.ToDto();
    }
}
