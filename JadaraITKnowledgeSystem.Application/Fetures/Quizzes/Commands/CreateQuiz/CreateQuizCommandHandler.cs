using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandHandler
    (IApplicationDbContext context,IFileManager fileManger,ILogger<CreateQuizCommandHandler> logger)
    : IRequestHandler<CreateQuizCommand, Result<QuizDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IFileManager _fileManager = fileManger;
    private readonly ILogger<CreateQuizCommandHandler> _logger = logger;

    public async Task<Result<QuizDto>> Handle(
        CreateQuizCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating quiz: {Title}", command.Title);

        var quizResult = Quiz.Create(
            courseId: command.CourseId,
            writerId: command.WriterId,
            title: command.Title,
            description: command.Description,
            tags: command.Tags
        );

        if (!quizResult.IsSuccess)
            return quizResult.Errors;

        var quiz = quizResult.Value;
        var uploadedFiles = new List<string>(); // Track for rollback

        try
        {
            foreach (var q in command.Questions)
            {
                // Move question image from temp to permanent if exists
                string? permanentQuestionImageUrl = null;
                if (!string.IsNullOrEmpty(q.ImageUrl))
                {
                    permanentQuestionImageUrl = await _fileManager.MoveFromTempToPermanentAsync(
                        q.ImageUrl,
                        "quizzes/questions",
                        cancellationToken
                    );
                    uploadedFiles.Add(permanentQuestionImageUrl);
                }

                var questionResult = Question.Create(
                    quizId: 0,
                    type: q.Type,
                    text: q.Text,
                    imageUrl: permanentQuestionImageUrl
                );

                if (!questionResult.IsSuccess)
                {
                    await RollbackUploadedFiles(uploadedFiles, cancellationToken);
                    return questionResult.Errors;
                }

                var question = questionResult.Value;
                quiz.AddQuestion(question);

                foreach (var c in q.Choices)
                {
                    // Move choice image from temp to permanent if exists
                    string? permanentChoiceImageUrl = null;
                    if (!string.IsNullOrEmpty(c.ImageUrl))
                    {
                        permanentChoiceImageUrl = await _fileManager.MoveFromTempToPermanentAsync(
                            c.ImageUrl,
                            "quizzes/choices",
                            cancellationToken
                        );
                        uploadedFiles.Add(permanentChoiceImageUrl);
                    }

                    var choiceResult = Choice.Create(
                        0,
                        c.Text,
                        c.IsCorrect,
                        permanentChoiceImageUrl
                    );

                    if (!choiceResult.IsSuccess)
                    {
                        await RollbackUploadedFiles(uploadedFiles, cancellationToken);
                        return choiceResult.Errors;
                    }

                    question.AddChoice(choiceResult.Value);
                }
            }

            await _context.Quizzes.AddAsync(quiz, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return quiz.ToDto();
        }
        catch
        {
            await RollbackUploadedFiles(uploadedFiles, cancellationToken);
            throw;
        }
    }

    private async Task RollbackUploadedFiles(List<string> fileUrls, CancellationToken cancellationToken)
    {
        foreach (var fileUrl in fileUrls)
        {
            try
            {
                await _fileManager.DeleteAsync(fileUrl, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete file during rollback: {FileUrl}", fileUrl);
            }
        }
    }
}