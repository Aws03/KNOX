using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseMaterial;

public sealed class CreateCourseMaterialCommandHandler(
    IApplicationDbContext context,
    IFeatureFlagService featureFlagService,
    ICurrentUserService currentUserService,
    IPostCommitDispatcher postCommitDispatcher,
    IFileManager files,
    ILogger<CreateCourseMaterialCommandHandler> logger)
    : IRequestHandler<CreateCourseMaterialCommand, Result<CourseMaterialDto>>
{
    private readonly ILogger<CreateCourseMaterialCommandHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    private readonly IFeatureFlagService _featureFlagService = featureFlagService;
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IPostCommitDispatcher _postCommitDispatcher = postCommitDispatcher;

    public async Task<Result<CourseMaterialDto>> Handle(
        CreateCourseMaterialCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating course material for CourseId={CourseId}, FolderId={FolderId}", request.CourseId, request.FolderId);

        var courseExists = await _context.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            _logger.LogWarning("Course not found. CourseId={CourseId}", request.CourseId);
            return Error.NotFound("Course.NotFound", $"Course with id {request.CourseId} not found");
        }

        if (request.FolderId.HasValue)
        {
            var folderCourseId = await _context.Folders
                .Where(f => f.Id == request.FolderId.Value)
                .Select(f => (int?)f.CourseId)
                .FirstOrDefaultAsync(cancellationToken);

            if (folderCourseId is null)
            {
                _logger.LogWarning("Folder not found. FolderId={FolderId}", request.FolderId);
                return Error.NotFound("Folder.NotFound", $"Folder with id {request.FolderId} not found");
            }
            if (folderCourseId != request.CourseId)
            {
                _logger.LogWarning("Folder does not belong to Course. FolderId={FolderId}, CourseId={CourseId}", request.FolderId, request.CourseId);
                return Error.Validation("Folder.CourseMismatch", "Specified folder does not belong to the given course.");
            }
        }

        // The browser uploaded the file straight to storage; verify it and move it under the course.
        var claimed = await files.ClaimMaterialUploadAsync(request.UploadKey, request.CourseId, cancellationToken);
        if (claimed.IsError)
            return claimed.Errors;
        var file = claimed.Value;

        // Create material (root if FolderId is null)
        var materialResult = CourseMaterial.Create(
            request.Title,
            file.Key,
            file.ContentType,
            file.Size,
            request.CourseId,
            request.FolderId,
            request.Description,
            request.Tags);

        if (materialResult.IsError)
        {
            await files.DeleteMaterialAsync(file.Key, CancellationToken.None);
            _logger.LogWarning("Validation errors creating material for CourseId={CourseId}: {Errors}", request.CourseId, materialResult.Errors);
            return materialResult.Errors;
        }

        var material = materialResult.Value;
        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course material created successfully. Id={Id} CourseId={CourseId}", material.Id, request.CourseId);

        if (request.GenerateQuiz)
            StageQuizGeneration(material.Id, request.QuizOptions ?? new QuizGenerationOptionsDto());

        return material.ToDto(files);
    }

    private void StageQuizGeneration(int materialId, QuizGenerationOptionsDto quizOptions)
    {
        if (!_featureFlagService.IsQuizGenerationEnabled())
        {
            _logger.LogWarning("Quiz generation requested but feature is disabled. MaterialId={MaterialId}", materialId);
            return;
        }

        if (_currentUserService.DomainUserId is not int writerId)
        {
            _logger.LogWarning("Quiz generation requested without an authenticated user. MaterialId={MaterialId}", materialId);
            return;
        }

        _logger.LogInformation("Initiating quiz generation for material. MaterialId={MaterialId}", materialId);

        // Stage quiz generation to run only after this command's transaction has
        // committed - DispatchPostCommitJobsBehavior hands this to the real
        // background queue once TransactionBehavior confirms the commit succeeded.
        var logger = _logger;
        _postCommitDispatcher.Enqueue(async (serviceProvider, ct) =>
        {
            try
            {
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new GenerateQuizFromMaterialCommand(materialId, writerId, quizOptions), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background quiz generation failed for material. MaterialId={MaterialId}", materialId);
            }
        });
    }
}
