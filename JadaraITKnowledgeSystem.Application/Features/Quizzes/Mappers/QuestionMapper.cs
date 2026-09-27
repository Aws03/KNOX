using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;

public static class QuestionMapper
{
    public static QuestionDto ToDto(this Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        return new QuestionDto
        {
            Id = question.Id,
            QuizId = question.QuizId,
            Type = question.Type,
            Text = question.Text,
            ImageUrl = question.ImageUrl,
            Choices = question.Choices.ToDtos() ?? new List<ChoiceDto>()
        };
    }

    public static List<QuestionDto> ToDtos(this IEnumerable<Question> questions)
    {
        return [.. questions.Select(q => q.ToDto())];
    }
}
