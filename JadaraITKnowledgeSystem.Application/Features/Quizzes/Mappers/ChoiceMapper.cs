using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;

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

    public static Choice ToEntity(this ChoiceDto choiceDto)
    {
        ArgumentNullException.ThrowIfNull(choiceDto);
        return Choice.Create(choiceDto.QuestionId, choiceDto.Text, choiceDto.IsCorrect, choiceDto.ImageUrl).Value;
        
    }

    public static List<Choice> ToEntities(this IEnumerable<ChoiceDto> choiceDtos)
    {
        ArgumentNullException.ThrowIfNull(choiceDtos);
        return [.. choiceDtos.Select(c => c.ToEntity())];
    }


}
