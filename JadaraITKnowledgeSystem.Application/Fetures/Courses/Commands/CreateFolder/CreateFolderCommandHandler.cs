using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Repositories;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateFolder
{
    public class CreateFolderCommandHandler
        (IApplicationDbContext context,ILogger<CreateFolderCommandHandler> logger)
        : IRequestHandler<CreateFolderCommand, Result<FolderDto>>
    {
        private readonly ILogger<CreateFolderCommandHandler> _logger = logger;
        private readonly IApplicationDbContext _context = context;

        public async Task<Result<FolderDto>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses
                .Include(c => c.Folders)
                .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);

            if (course == null)
                return Error.NotFound($"Course with id {request.CourseId} not found");

            var folderResult = course.AddFolder(request.Name, request.ParentFolderId, request.Description);
            if (folderResult.IsError)
            {
                return folderResult.Errors;
            }

            var saveResult = await _context.SaveChangesAsync(cancellationToken) > 0;
            if (_logger.IsEnabled(LogLevel.Information) && saveResult)
            {
                _logger.LogInformation("Folder {FolderName} created successfully in course {CourseId}", request.Name, request.CourseId);
            }
            else if (_logger.IsEnabled(LogLevel.Warning) && !saveResult)
            {
                _logger.LogWarning("Failed to create folder {FolderName} in course {CourseId}", request.Name, request.CourseId);
            }

            return folderResult.Value.ToDto();
        }
    }
}
