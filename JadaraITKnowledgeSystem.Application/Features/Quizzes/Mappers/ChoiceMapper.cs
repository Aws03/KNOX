using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Mappers;

public static class ChoiceMapper
{
    public static ChoiceDto ToDto(this Choice choice)
    {
        return new ChoiceDto
        {
            Id = choice.Id,
            Text = choice.Text,
            ImageUrl = choice.ImageUrl,
            IsCorrect = choice.IsCorrect
        };
    }

    public static List<ChoiceDto> ToDtos(this IEnumerable<Choice> choices)
    {
        return [.. choices.Select(c => c.ToDto())];
    }
}
