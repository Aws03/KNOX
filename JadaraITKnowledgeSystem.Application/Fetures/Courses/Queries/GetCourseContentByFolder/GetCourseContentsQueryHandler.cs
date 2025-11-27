using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseContentByFolder
{
    public class GetCourseContentsQueryHandler
        (IApplicationDbContext context, ILogger<GetCourseContentsQueryHandler> logger)
        : IRequestHandler<GetCourseContentsQuery, Result<CourseContentsDto>>
    {
        private readonly IApplicationDbContext _context = context;
        private readonly ILogger<GetCourseContentsQueryHandler> _logger = logger;

        public async Task<Result<CourseContentsDto>> Handle(
            GetCourseContentsQuery request,
            CancellationToken ct)
        {
            // Validate course exists
            var courseExists = await _context.Courses
                .AnyAsync(c => c.Id == request.CourseId, ct);

            if (!courseExists)
                return Error.NotFound("Course.NotFound", $"Course {request.CourseId} not found");

            // Load folders for this level
            var folders = await _context.Folders
                .Where(f => f.CourseId == request.CourseId &&
                            f.ParentFolderId == request.FolderId)
                .Select(f => f.ToDto())
                .ToListAsync(ct);

            // Load materials for this level
            var materials = await _context.CourseMaterials
                .Where(m => m.CourseId == request.CourseId &&
                            m.FolderId == request.FolderId)
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
