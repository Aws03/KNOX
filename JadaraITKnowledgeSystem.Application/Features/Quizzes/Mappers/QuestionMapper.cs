using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;

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

    public static Question ToEntity(this QuestionDto questionDto)
    {
        ArgumentNullException.ThrowIfNull(questionDto);

        var question = Question.Create(
            questionDto.QuizId,
            (Domain.Quizzes.Enums.QuestionType)questionDto.Type,
            questionDto.Text
        ).Value;

        foreach (var choiceDto in questionDto.Choices)
        {
            var choice = choiceDto.ToEntity();
            question.AddChoice(choice);
        }

        return question;
    }

    public static List<Question> ToEntities(this IEnumerable<QuestionDto> questionDtos)
    {
        return [.. questionDtos.Select(q => q.ToEntity())];
    }
}
