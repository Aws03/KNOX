using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers
{
    public static class ChoiceMapper
    {
        public static ChoiceDto ToDto(this Choice choice)
        {
            return new ChoiceDto
            {
                Id = choice.Id,
                Text = choice.Text,
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
            return Choice.Create(choiceDto.QuestionId, choiceDto.Text, choiceDto.IsCorrect).Value;
            
        }

        public static List<Choice> ToEntities(this IEnumerable<ChoiceDto> choiceDtos)
        {
            ArgumentNullException.ThrowIfNull(choiceDtos);
            return [.. choiceDtos.Select(c => c.ToEntity())];
        }


    }
}
