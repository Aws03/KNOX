using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;

public static class QuizGenerationJobMapper
{
    public static QuizGenerationJobDto ToDto(this QuizGenerationJob job)
    {
        return new QuizGenerationJobDto
        {
            Id = job.Id,
            MaterialId = job.MaterialId,
            MaterialTitle = job.Material?.Title ?? "",
            CourseId = job.CourseId,
            CourseName = job.Course?.CourseName ?? "",
            Status = job.Status,
            GeneratedQuizCount = job.GeneratedQuizCount,
            GeneratedQuizIds = job.GetGeneratedQuizIds(),
            ErrorMessage = job.ErrorMessage,
            CreatedAt = job.CreatedAt.UtcDateTime,
            CompletedAt = job.CompletedAt,
            Options = MapOptions(job.GetOptions())
        };
    }

    private static QuizGenerationOptionsDto MapOptions(Domain.Quizzes.Entities.QuizGenerationOptions options)
    {
        return new QuizGenerationOptionsDto
        {
            QuestionsPerQuiz = options.QuestionsPerQuiz,
            Difficulty = options.Difficulty,
            MaxQuizzes = options.MaxQuizzes,
            AutoPublish = options.AutoPublish
        };
    }
}
