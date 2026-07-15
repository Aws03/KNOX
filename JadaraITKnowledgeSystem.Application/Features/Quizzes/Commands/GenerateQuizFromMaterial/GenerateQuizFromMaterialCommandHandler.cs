using System;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.ProcessQuizGenerationJob;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;

public sealed class GenerateQuizFromMaterialCommandHandler
    : IRequestHandler<GenerateQuizFromMaterialCommand, Result<QuizGenerationJobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IFeatureFlagService _featureFlagService;
    private readonly IPostCommitDispatcher _postCommitDispatcher;
    private readonly ILogger<GenerateQuizFromMaterialCommandHandler> _logger;

    public GenerateQuizFromMaterialCommandHandler(
        IApplicationDbContext context,
        IFeatureFlagService featureFlagService,
        IPostCommitDispatcher postCommitDispatcher,
        ILogger<GenerateQuizFromMaterialCommandHandler> logger)
    {
        _context = context;
        _featureFlagService = featureFlagService;
        _postCommitDispatcher = postCommitDispatcher;
        _logger = logger;
    }

    public async Task<Result<QuizGenerationJobDto>> Handle(
        GenerateQuizFromMaterialCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Quiz generation requested. MaterialId={MaterialId}, WriterId={WriterId}",
            request.MaterialId, request.WriterId);

        // Check feature flag
        if (!_featureFlagService.IsQuizGenerationEnabled())
        {
            _logger.LogWarning("Quiz generation feature is disabled");
            return Error.Validation(
                "QuizGeneration.Disabled",
                "Quiz generation feature is currently disabled");
        }

        // Validate material exists
        var material = await _context.CourseMaterials
            .Include(m => m.Course)
            .FirstOrDefaultAsync(m => m.Id == request.MaterialId, cancellationToken);

        if (material == null)
        {
            _logger.LogWarning("Material not found. MaterialId={MaterialId}", request.MaterialId);
            return Error.NotFound(
                "Material.NotFound",
                $"Material with ID {request.MaterialId} not found");
        }

        // Check if material supports text extraction
        if (!material.SupportsTextExtraction())
        {
            _logger.LogWarning(
                "Material does not support text extraction. MaterialId={MaterialId}, ContentUrl={ContentUrl}",
                request.MaterialId, material.ContentUrl);
            return Error.Validation(
                "QuizGeneration.UnsupportedFile",
                "File type does not support text extraction. Supported types: PDF, DOCX, PPTX");
        }

        // Create quiz generation job
        var options = new QuizGenerationOptions
        {
            QuestionsPerQuiz = request.Options.QuestionsPerQuiz,
            Difficulty = request.Options.Difficulty,
            MaxQuizzes = request.Options.MaxQuizzes,
            AutoPublish = request.Options.AutoPublish
        };

        var jobResult = QuizGenerationJob.Create(
            request.MaterialId,
            material.CourseId,
            request.WriterId,
            options);

        if (jobResult.IsError)
        {
            _logger.LogWarning(
                "Failed to create quiz generation job: {Errors}",
                jobResult.Errors);
            return jobResult.Errors;
        }

        var job = jobResult.Value;
        _context.QuizGenerationJobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Quiz generation job created. JobId={JobId}, MaterialId={MaterialId}",
            job.Id, request.MaterialId);

        // Capture the job ID for background processing
        var jobId = job.Id;

        // Stage the actual processing to run only after this command's transaction
        // has committed - DispatchPostCommitJobsBehavior hands this to the real
        // background queue once TransactionBehavior confirms the commit succeeded.
        _postCommitDispatcher.Enqueue(async (serviceProvider, ct) =>
        {
            try
            {
                var mediator = serviceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new ProcessQuizGenerationJobCommand(jobId), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background quiz generation failed. JobId={JobId}", jobId);
            }
        });

        return job.ToDto();
    }
}
