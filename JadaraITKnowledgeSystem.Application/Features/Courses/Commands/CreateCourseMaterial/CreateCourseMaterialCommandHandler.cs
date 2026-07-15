using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;


namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseMaterial;

public sealed class CreateCourseMaterialCommandHandler(
    IApplicationDbContext context,
    IFeatureFlagService featureFlagService,
    ICurrentUserService currentUserService,
    IPostCommitDispatcher postCommitDispatcher,
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
            request.Description,
            request.Tags);

        if (materialResult.IsError)
        {
            _logger.LogWarning("Validation errors creating material for CourseId={CourseId}: {Errors}", request.CourseId, materialResult.Errors);
            return materialResult.Errors;
        }

        var material = materialResult.Value;
        _context.CourseMaterials.Add(material);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Course material created successfully. Id={Id} CourseId={CourseId}", material.Id, request.CourseId);

        // Capture material ID for background processing
        var materialId = material.Id;
        var userId = _currentUserService.UserId ?? 1;

        // Auto-generate quiz if requested and feature is enabled
        if (request.GenerateQuiz && _featureFlagService.IsQuizGenerationEnabled())
        {
            _logger.LogInformation("Initiating quiz generation for material. MaterialId={MaterialId}", materialId);

            // Stage quiz generation to run only after this command's transaction has
            // committed - DispatchPostCommitJobsBehavior hands this to the real
            // background queue once TransactionBehavior confirms the commit succeeded.
            _postCommitDispatcher.Enqueue(async (serviceProvider, ct) =>
            {
                try
                {
                    var mediator = serviceProvider.GetRequiredService<IMediator>();

                    var quizOptions = request.QuizOptions ?? new QuizGenerationOptionsDto();
                    var generateCommand = new GenerateQuizFromMaterialCommand(
                        materialId,
                        userId,
                        quizOptions);

                    await mediator.Send(generateCommand, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Background quiz generation failed for material. MaterialId={MaterialId}",
                        materialId);
                }
            });
        }
        else if (request.GenerateQuiz && !_featureFlagService.IsQuizGenerationEnabled())
        {
            _logger.LogWarning("Quiz generation requested but feature is disabled. MaterialId={MaterialId}", materialId);
        }

        return material.ToDto();
    }
}
