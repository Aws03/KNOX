using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.ProcessQuizGenerationJob;

public sealed class ProcessQuizGenerationJobCommandHandler 
    : IRequestHandler<ProcessQuizGenerationJobCommand, Result<List<QuizDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ITextExtractionService _textExtractor;
    private readonly IOpenAIService _openAI;
    private readonly IFileManager _files;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessQuizGenerationJobCommandHandler> _logger;

    public ProcessQuizGenerationJobCommandHandler(
        IApplicationDbContext context,
        ITextExtractionService textExtractor,
        IOpenAIService openAI,
        IFileManager files,
        IMediator mediator,
        ILogger<ProcessQuizGenerationJobCommandHandler> logger)
    {
        _context = context;
        _textExtractor = textExtractor;
        _openAI = openAI;
        _files = files;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<List<QuizDto>>> Handle(
        ProcessQuizGenerationJobCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing quiz generation job. JobId={JobId}", request.JobId);

        // Load job and related entities
        var job = await _context.QuizGenerationJobs
            .Include(j => j.Material)
            .Include(j => j.Course)
            .FirstOrDefaultAsync(j => j.Id == request.JobId, cancellationToken);

        if (job == null)
        {
            _logger.LogWarning("Quiz generation job not found. JobId={JobId}", request.JobId);
            return Error.NotFound("QuizGenerationJob.NotFound", $"Job with ID {request.JobId} not found");
        }

        var material = job.Material;
        if (material == null)
        {
            job.MarkFailed("Associated material not found");
            await _context.SaveChangesAsync(cancellationToken);
            return Error.NotFound("Material.NotFound", "Associated material not found");
        }

        try
        {
            // Step 1: Update status to Extracting
            job.UpdateStatus(QuizGenerationStatus.Extracting);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 2: Download and extract text from material
            var extractionResult = await ExtractTextFromMaterial(material, cancellationToken);
            if (extractionResult.IsError)
            {
                job.MarkFailed($"Text extraction failed: {string.Join(", ", extractionResult.Errors.Select(e => e.Description))}");
                await _context.SaveChangesAsync(cancellationToken);
                return extractionResult.Errors;
            }

            var extractedText = extractionResult.Value;
            _logger.LogInformation(
                "Text extracted successfully. JobId={JobId}, TextLength={Length}",
                job.Id, extractedText.Length);

            var options = job.GetOptions();

            // Step 3: Chunk text intelligently
            var chunks = await _textExtractor.ChunkTextIntelligentlyAsync(
                extractedText,
                new ChunkingOptions(),
                cancellationToken);

            // Limit chunks based on job options
            var maxChunks = Math.Min(chunks.Count, options.MaxQuizzes);
            _logger.LogInformation(
                "Text chunked into {ChunkCount} chunks, processing {MaxChunks} quizzes",
                chunks.Count, maxChunks);

            // Step 4: Update status to GeneratingQuizzes
            job.UpdateStatus(QuizGenerationStatus.GeneratingQuizzes);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 5: Generate quizzes for each chunk
            var createdQuizzes = new List<QuizDto>();
            Error? firstError = null;
            for (int i = 0; i < maxChunks; i++)
            {
                var quizResult = await GenerateQuizFromChunk(
                    chunks[i],
                    material,
                    job,
                    options,
                    i + 1,
                    maxChunks,
                    cancellationToken);

                if (quizResult.IsSuccess)
                {
                    createdQuizzes.Add(quizResult.Value);
                    job.AddGeneratedQuizId(quizResult.Value.Id);
                    _logger.LogInformation(
                        "Quiz generated successfully. JobId={JobId}, QuizId={QuizId}, Part={Part}/{Total}",
                        job.Id, quizResult.Value.Id, i + 1, maxChunks);
                }
                else
                {
                    firstError ??= quizResult.TopError;
                    _logger.LogWarning(
                        "Failed to generate quiz for chunk {Index}: {Errors}",
                        i + 1, string.Join(", ", quizResult.Errors.Select(e => e.Description)));
                }
            }

            // Step 6: Mark job completed or failed
            if (createdQuizzes.Count == 0)
            {
                // Surface the cause (e.g. OpenAI.NotConfigured) instead of a generic message.
                job.MarkFailed(firstError is null
                    ? "No quizzes were successfully generated"
                    : $"No quizzes were generated: {firstError.Value.Description}");
                await _context.SaveChangesAsync(cancellationToken);
                return Error.Failure("QuizGeneration.NoResults", "Failed to generate any quizzes");
            }

            job.MarkCompleted(createdQuizzes.Count);
            material.MarkQuizzesGenerated(createdQuizzes.Count);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Quiz generation completed successfully. JobId={JobId}, GeneratedQuizzes={Count}",
                job.Id, createdQuizzes.Count);

            return createdQuizzes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during quiz generation. JobId={JobId}", job.Id);
            job.MarkFailed($"Unexpected error: {ex.Message}");
            await _context.SaveChangesAsync(cancellationToken);
            return Error.Failure("QuizGeneration.UnexpectedError", $"Unexpected error: {ex.Message}");
        }
    }

    private async Task<Result<string>> ExtractTextFromMaterial(
        CourseMaterial material,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Downloading material file {StorageKey}", material.StorageKey);

            await using var remote = await _files.OpenMaterialAsync(material.StorageKey, cancellationToken);
            if (remote == null)
            {
                return Error.NotFound("File.NotFound", "Material file not found in storage");
            }

            // Only documents reach this point (SupportsTextExtraction); their readers need a seekable stream.
            await using var fileStream = new MemoryStream();
            await remote.CopyToAsync(fileStream, cancellationToken);
            fileStream.Position = 0;

            var extension = Path.GetExtension(material.StorageKey);
            _logger.LogInformation("Extracting text from file. Extension={Extension}", extension);

            return await _textExtractor.ExtractTextAsync(fileStream, extension, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from material. MaterialId={MaterialId}", material.Id);
            return Error.Failure("TextExtraction.Failed", $"Failed to extract text: {ex.Message}");
        }
    }

    private async Task<Result<QuizDto>> GenerateQuizFromChunk(
        TextChunk chunk,
        CourseMaterial material,
        QuizGenerationJob job,
        QuizGenerationOptions options,
        int partNumber,
        int totalParts,
        CancellationToken cancellationToken)
    {
        try
        {
            // Generate quiz using OpenAI
            var aiRequest = new GenerateQuizRequest
            {
                Text = chunk.Text,
                QuestionCount = options.QuestionsPerQuiz,
                Difficulty = options.Difficulty,
                ChunkIndex = partNumber - 1,
                TotalChunks = totalParts
            };

            var aiQuizResult = await _openAI.GenerateQuizFromTextAsync(aiRequest, cancellationToken);
            if (aiQuizResult.IsError)
            {
                return aiQuizResult.Errors;
            }

            var aiQuiz = aiQuizResult.Value;

            // Generate title
            var title = GenerateQuizTitle(material, partNumber, totalParts, aiQuiz.Topic);

            // Generate description
            var description = string.IsNullOrEmpty(aiQuiz.Description)
                ? GenerateFallbackDescription(material, partNumber, totalParts)
                : aiQuiz.Description;

            // Merge tags
            var tags = MergeTags(
                material.Tags,
                aiQuiz.SuggestedTags,
                new[] { "auto-generated", $"material-{material.Id}" });

            // Create quiz using existing command
            var createQuizCommand = new CreateQuizCommand(
                Title: title,
                WriterId: job.RequestedByUserId,
                CourseId: material.CourseId,
                Description: description,
                Questions: aiQuiz.Questions,
                Tags: tags
            );

            var quizResult = await _mediator.Send(createQuizCommand, cancellationToken);
            if (quizResult.IsError)
                return quizResult;

            var quiz = await _context.Quizzes.FindAsync([quizResult.Value.Id], cancellationToken);
            if (quiz is not null)
            {
                var markResult = quiz.MarkAsAiGenerated(material.Id, partNumber, totalParts);
                if (markResult.IsError)
                    return markResult.Errors;

                await _context.SaveChangesAsync(cancellationToken);
            }

            return quizResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating quiz from chunk. JobId={JobId}, Part={Part}", job.Id, partNumber);
            return Error.Failure("QuizGeneration.Failed", $"Failed to generate quiz: {ex.Message}");
        }
    }

    private static string GenerateQuizTitle(
        CourseMaterial material,
        int partNumber,
        int totalParts,
        string? topic)
    {
        var title = totalParts > 1
            ? $"{material.Title} - Part {partNumber}/{totalParts}"
            : material.Title;

        if (!string.IsNullOrEmpty(topic))
        {
            title += $" - {topic}";
        }

        return title.Length > 250 ? title.Substring(0, 247) + "..." : title;
    }

    private static string GenerateFallbackDescription(
        CourseMaterial material,
        int partNumber,
        int totalParts)
    {
        var desc = totalParts > 1
            ? $"Auto-generated quiz from '{material.Title}' (Part {partNumber} of {totalParts}). "
            : $"Auto-generated quiz from '{material.Title}'. ";

        desc += "Test your understanding of the key concepts covered in this educational material.";

        return desc.Length > 500 ? desc.Substring(0, 497) + "..." : desc;
    }

    private static List<string> MergeTags(
        IEnumerable<string>? materialTags,
        IEnumerable<string>? aiTags,
        IEnumerable<string> metadataTags)
    {
        var tags = new List<string>();

        if (materialTags != null)
            tags.AddRange(materialTags.Take(5));

        if (aiTags != null)
            tags.AddRange(aiTags.Take(3));

        tags.AddRange(metadataTags);

        return tags.Distinct()
                   .Where(t => !string.IsNullOrWhiteSpace(t))
                   .Take(10)
                   .ToList();
    }
}
