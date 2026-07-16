using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteFolder;

public sealed class DeleteFolderCommandHandler
    (IApplicationDbContext context, ILogger<DeleteFolderCommandHandler> logger)
    : IRequestHandler<DeleteFolderCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<DeleteFolderCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(
        DeleteFolderCommand request,
        CancellationToken cancellationToken)
    {
        var folder = await _context.Folders
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == request.FolderId, cancellationToken);

        if (folder is null)
        {
            _logger.LogWarning("Folder with ID {FolderId} not found.", request.FolderId);
            return Error.NotFound("Folder.NotFound", $"Folder with ID {request.FolderId} was not found.");
        }

        var course = await _context.Courses
            .Include(c => c.Folders)
            .Include(c => c.Materials)
            .FirstOrDefaultAsync(c => c.Id == folder.CourseId, cancellationToken);

        if (course is null)
        {
            _logger.LogWarning("Course with ID {CourseId} not found.", folder.CourseId);
            return Error.NotFound("Course.NotFound", $"Course with ID {folder.CourseId} was not found.");
        }

        var removeResult = course.RemoveFolder(request.FolderId, request.DeleteContents);
        if (removeResult.IsError)
            return removeResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Folder {FolderId} deleted successfully.", request.FolderId);

        return Result.Success;
    }
}
