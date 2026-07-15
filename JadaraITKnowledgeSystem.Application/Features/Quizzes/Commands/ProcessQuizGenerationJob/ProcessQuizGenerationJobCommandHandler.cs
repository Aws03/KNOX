using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
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
    private readonly IStorageService _storage;
    private readonly IMediator _mediator;
    private readonly ILogger<ProcessQuizGenerationJobCommandHandler> _logger;

    public ProcessQuizGenerationJobCommandHandler(
        IApplicationDbContext context,
        ITextExtractionService textExtractor,
        IOpenAIService openAI,
        IStorageService storage,
        IMediator mediator,
        ILogger<ProcessQuizGenerationJobCommandHandler> logger)
    {
        _context = context;
        _textExtractor = textExtractor;
        _openAI = openAI;
        _storage = storage;
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

            // Step 3: Chunk text intelligently
            var chunks = await _textExtractor.ChunkTextIntelligentlyAsync(
                extractedText,
                new ChunkingOptions
                {
                    MaxCharsPerChunk = 4000,
                    QuestionsPerChunk = job.GetOptions().QuestionsPerQuiz
                },
                cancellationToken);

            // Limit chunks based on job options
            var maxChunks = Math.Min(chunks.Count, job.GetOptions().MaxQuizzes);
            _logger.LogInformation(
                "Text chunked into {ChunkCount} chunks, processing {MaxChunks} quizzes",
                chunks.Count, maxChunks);

            // Step 4: Update status to GeneratingQuizzes
            job.UpdateStatus(QuizGenerationStatus.GeneratingQuizzes);
            await _context.SaveChangesAsync(cancellationToken);

            // Step 5: Generate quizzes for each chunk
            var createdQuizzes = new List<QuizDto>();
            for (int i = 0; i < maxChunks; i++)
            {
                var quizResult = await GenerateQuizFromChunk(
                    chunks[i],
                    material,
                    job,
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
                    _logger.LogWarning(
                        "Failed to generate quiz for chunk {Index}: {Errors}",
                        i + 1, string.Join(", ", quizResult.Errors.Select(e => e.Description)));
                }
            }

            // Step 6: Mark job completed or failed
            if (createdQuizzes.Count == 0)
            {
                job.MarkFailed("No quizzes were successfully generated");
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
        Domain.Courses.Entites.CourseMaterial material,
        CancellationToken cancellationToken)
    {
        try
        {
            // The ContentUrl is either a CDN URL (e.g. https://jadara-hub.b-cdn.net/permanent/Lesson/file.pdf,
            // storage path = everything after the domain) or a LocalFileStorage URL
            // (e.g. http://host/uploads/permanent/Lesson/file.pdf, storage path = everything
            // after the leading "uploads" segment, since LocalFileStorage's root already is
            // wwwroot/uploads). Strip that leading segment when present so both shapes resolve
            // to the same on-disk folder LocalFileStorage.DownloadAsync expects.
            var uri = new Uri(material.ContentUrl);

            // Split the path into segments
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length > 0 && segments[0].Equals("uploads", StringComparison.OrdinalIgnoreCase))
            {
                segments = segments[1..];
            }

            if (segments.Length == 0)
            {
                return Error.Validation("InvalidUrl", "Unable to parse file path from URL");
            }

            var fileName = segments[^1]; // Last segment is always the file name

            // Everything except the last segment is the folder path
            // For URL: https://jadara-hub.b-cdn.net/permanent/Lesson/file.pdf
            // segments = ["permanent", "Lesson", "file.pdf"]
            // folder should be: "permanent/Lesson"
            var folder = segments.Length > 1
                ? string.Join("/", segments[..^1])
                : null;

            _logger.LogInformation("Downloading file from storage. FileName={FileName}, Folder={Folder}", fileName, folder);

            // Download file from storage
            var fileStream = await _storage.DownloadAsync(fileName, folder, cancellationToken);
            if (fileStream == null)
            {
                return Error.NotFound("File.NotFound", "Material file not found in storage");
            }

            var extension = Path.GetExtension(fileName);
            _logger.LogInformation("Extracting text from file. Extension={Extension}", extension);

            // Extract text
            var extractionResult = await _textExtractor.ExtractTextAsync(fileStream, extension, cancellationToken);
            
            if (fileStream != null)
            {
                await fileStream.DisposeAsync();
            }

            return extractionResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting text from material. MaterialId={MaterialId}", material.Id);
            return Error.Failure("TextExtraction.Failed", $"Failed to extract text: {ex.Message}");
        }
    }

    private async Task<Result<QuizDto>> GenerateQuizFromChunk(
        TextChunk chunk,
        Domain.Courses.Entites.CourseMaterial material,
        Domain.Quizzes.Entities.QuizGenerationJob job,
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
                QuestionCount = job.GetOptions().QuestionsPerQuiz,
                Difficulty = job.GetOptions().Difficulty,
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

            if (quizResult.IsSuccess)
            {
                // Update the quiz to mark it as AI-generated
                var quiz = await _context.Quizzes.FindAsync(quizResult.Value.Id);
                if (quiz != null)
                {
                    // You'll need to add a method to Quiz entity to update source
                    // For now, this will be handled in the database update
                }
            }

            return quizResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating quiz from chunk. JobId={JobId}, Part={Part}", job.Id, partNumber);
            return Error.Failure("QuizGeneration.Failed", $"Failed to generate quiz: {ex.Message}");
        }
    }

    private string GenerateQuizTitle(
        Domain.Courses.Entites.CourseMaterial material,
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

    private string GenerateFallbackDescription(
        Domain.Courses.Entites.CourseMaterial material,
        int partNumber,
        int totalParts)
    {
        var desc = totalParts > 1
            ? $"Auto-generated quiz from '{material.Title}' (Part {partNumber} of {totalParts}). "
            : $"Auto-generated quiz from '{material.Title}'. ";

        desc += "Test your understanding of the key concepts covered in this educational material.";

        return desc.Length > 500 ? desc.Substring(0, 497) + "..." : desc;
    }

    private List<string> MergeTags(
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
