using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.Logging;


namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial;

public sealed class CreateCourseMaterialCommandHandler(
    IApplicationDbContext context,
    ILogger<CreateCourseMaterialCommandHandler> logger)
    : IRequestHandler<CreateCourseMaterialCommand, Result<CourseMaterialDto>>
{
    private readonly ILogger<CreateCourseMaterialCommandHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<CourseMaterialDto>> Handle(
        CreateCourseMaterialCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating course material for CourseId={CourseId}, FolderId={FolderId}", request.CourseId, request.FolderId);

        // Validate Course existence
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
        if (course == null)
        {
            _logger.LogWarning("Course not found. CourseId={CourseId}", request.CourseId);
            return Error.NotFound("Course.NotFound", $"Course with id {request.CourseId} not found");
        }

        Folder? folder = null;
        if (request.FolderId.HasValue)
        {
            folder = await _context.Folders
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == request.FolderId.Value, cancellationToken);
            if (folder == null)
            {
                _logger.LogWarning("Folder not found. FolderId={FolderId}", request.FolderId);
                return Error.NotFound("Folder.NotFound", $"Folder with id {request.FolderId} not found");
            }
            if (folder.CourseId != request.CourseId)
            {
                _logger.LogWarning("Folder does not belong to Course. FolderId={FolderId}, CourseId={CourseId}", request.FolderId, request.CourseId);
                return Error.Validation("Folder.CourseMismatch", "Specified folder does not belong to the given course.");
            }
        }

        // Create material (root if FolderId is null)
        var materialResult = CourseMaterial.Create(
            request.Title,
            request.ContemtUrl,
            request.CourseId,
            request.FolderId,
            request.Description);

        if (materialResult.IsError)
        {
            _logger.LogWarning("Validation errors creating material for CourseId={CourseId}: {Errors}", request.CourseId, materialResult.Errors);
            return materialResult.Errors;
        }

        var material = materialResult.Value;
        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course material created successfully. Id={Id} CourseId={CourseId}", material.Id, request.CourseId);
        return material.ToDto();
    }
}
