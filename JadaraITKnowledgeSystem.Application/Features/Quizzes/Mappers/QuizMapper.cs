using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;

public static class QuizMapper
{
    public static QuizDto ToDto(this Quiz quiz)
    {
        ArgumentNullException.ThrowIfNull(quiz);

        return new QuizDto
        {
            Id = quiz.Id,
            CourseId = quiz.CourseId,
            WriterId = quiz.WriterId,
            WriterName = quiz.Writer?.Name.Value ?? "Unknown",
            Title = quiz.Title,
            Description = quiz.Description,
            Likes = quiz.Likes,
            Dislikes = quiz.Dislikes,
            CreatedAt = quiz.CreatedAt,
            Questions = quiz.Questions?.ToDtos() ?? new List<QuestionDto>(),
            Tags = quiz.Tags.ToList()
        };
    }

    public static List<QuizDto> ToDtos(this IEnumerable<Quiz> quizzes)
    {
        ArgumentNullException.ThrowIfNull(quizzes);
        return quizzes.Select(q => q.ToDto()).ToList();
    }
}
