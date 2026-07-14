using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetCourseContentsByWriterId
{
    public class GetCourseContentsByWriterIdQueryHandler
        (IApplicationDbContext context, ICurrentUserService currentUser, ILogger<GetCourseContentsByWriterIdQueryHandler> logger)
        : IRequestHandler<GetCourseContentsByWriterIdQuery, Result<CourseContentsDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly ILogger<GetCourseContentsByWriterIdQueryHandler> _logger = logger;

        public async Task<Result<CourseContentsDto>> Handle(
            GetCourseContentsByWriterIdQuery request,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId;
            if (!userId.HasValue)
                return Error.Unauthorized("User.NotAuthenticated", "User is not authenticated.");

            // Validate course exists
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == request.CourseId, ct);

            if (!courseExists)
                return Error.NotFound("Course.NotFound", $"Course {request.CourseId} not found");

            // Get user's email from Identity
            var userEmail = _currentUser.Email;
            if (string.IsNullOrWhiteSpace(userEmail))
                return Error.Unauthorized("User.NotAuthenticated", "User email not found.");

            _logger.LogInformation("Getting course contents for Course {CourseId}, Writer {Email}", request.CourseId, userEmail);

            // Load folders for this level created by current user
            var folders = await _context.Folders
                .Where(f => f.CourseId == request.CourseId &&
                            f.ParentFolderId == request.FolderId &&
                            f.CreatedBy == userEmail)
                .Select(f => f.ToDto())
                .ToListAsync(ct);

            // Load materials for this level created by current user
            var materials = await _context.CourseMaterials
                .Where(m => m.CourseId == request.CourseId &&
                            m.FolderId == request.FolderId &&
                            m.CreatedBy == userEmail)
                .Select(m => m.ToDto())
                .ToListAsync(ct);

            var result = new CourseContentsDto(
                CourseId: request.CourseId,
                FolderId: request.FolderId,
                Folders: folders,
                Materials: materials
            );

            return result;
        }
    }
}
