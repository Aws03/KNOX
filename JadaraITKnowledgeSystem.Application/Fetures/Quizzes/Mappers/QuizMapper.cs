using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers;

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
            Title = quiz.Title,
            Description = quiz.Description,
            Likes = quiz.Likes,
            Dislikes = quiz.Dislikes,
            Questions = quiz.Questions?.ToDtos() ?? new List<QuestionDto>()
        };
    }

    public static List<QuizDto> ToDtos(this IEnumerable<Quiz> quizzes)
    {
        ArgumentNullException.ThrowIfNull(quizzes);
        return quizzes.Select(q => q.ToDto()).ToList();
    }

    public static QuizSummaryDto ToSummaryDto(this Quiz quiz)
    {
        ArgumentNullException.ThrowIfNull(quiz);

        return new QuizSummaryDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Likes = quiz.Likes,
            WriterName = quiz.Writer?.Name.Value ?? "Unknown",
            CreatedAt = quiz.CreatedAt
        };
    }

    public static List<QuizSummaryDto> ToSummaryDtos(this IEnumerable<Quiz> quizzes)
    {
        return quizzes.Select(q => q.ToSummaryDto()).ToList();
    }

    public static Quiz ToEntity(this QuizDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var quiz =  Quiz.Create(dto.CourseId,dto.WriterId, dto.Title, dto.Description).Value;

        if(dto.Questions is not null && dto.Questions.Any())
        {
            foreach (var questionDto in dto.Questions)
            {
                var question = questionDto.ToEntity();
                quiz.AddQuestion(question);
            }
        }

        return quiz;

    }

    public static List<Quiz> ToEntities(this IEnumerable<QuizDto> dtos)
    {
        ArgumentNullException.ThrowIfNull(dtos);
        return dtos.Select(d => d.ToEntity()).ToList();
    }
}
