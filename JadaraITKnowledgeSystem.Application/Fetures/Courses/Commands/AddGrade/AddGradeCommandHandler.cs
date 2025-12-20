// TODO: Grade functionality is temporarily disabled.
// Universities may have different grading systems (A, A+, B, etc.)
// This needs to be redesigned to support flexible grading systems.

/*
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.AddGrade;

public sealed class AddGradeCommandHandler(
    IApplicationDbContext context,
    ICurrentUserService currentUserService,
    ILogger<AddGradeCommandHandler> logger)
    : IRequestHandler<AddGradeCommand, Result<EnrollmentDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly ILogger<AddGradeCommandHandler> _logger = logger;

    public async Task<Result<EnrollmentDto>> Handle(AddGradeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling AddGrade: CourseId={CourseId}, Grade={Grade}",
            request.CourseId, request.Grade);

        // Get current user
        var userId = _currentUserService.UserId;
        if (!userId.HasValue || userId.Value <= 0)
        {
            _logger.LogWarning("AddGrade failed: User not authenticated");
            return Error.Unauthorized("User.NotAuthenticated", "User must be authenticated to add a grade.");
        }

        // Find enrollment
        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.UserId == userId.Value && e.CourseId == request.CourseId, cancellationToken);

        if (enrollment == null)
        {
            _logger.LogWarning("AddGrade failed: Enrollment not found, UserId={UserId}, CourseId={CourseId}",
                userId.Value, request.CourseId);
            return Error.NotFound("Enrollment.NotFound", "User is not enrolled in this course.");
        }

        // Set grade
        var result = enrollment.SetGrade(request.Grade);
        if (result.IsError)
        {
            _logger.LogWarning("AddGrade domain operation failed: {@Errors}", result.Errors);
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AddGrade completed: EnrollmentId={EnrollmentId}, UserId={UserId}, CourseId={CourseId}, Grade={Grade}",
            enrollment.Id, userId.Value, request.CourseId, enrollment.Grade);

        return enrollment.ToDto();
    }
}
*/
