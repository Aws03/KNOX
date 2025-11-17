using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Mappers
{
    public static class UserReactionMapper
    {

        public static UserReactionDto ToDto(this UserReaction userReaction)
        {
            ArgumentNullException.ThrowIfNull(userReaction);

            return new UserReactionDto
            {
                Id = userReaction.Id,
                QuizId = userReaction.QuizId,
                UserId = userReaction.UserId,
                ReactionType = userReaction.ReactionType,
            };
        }

        public static List<UserReactionDto> ToDtos(this IEnumerable<UserReaction> userReactions)
        {
            var dtoList = userReactions.Select(userReactions => userReactions.ToDto()).ToList();
            return dtoList;
        }

        public static UserReaction  ToEntity(this UserReactionDto userReactionDto)
        {
            ArgumentNullException.ThrowIfNull(userReactionDto);

            return UserReaction.Create(userReactionDto.UserId,userReactionDto.QuizId,userReactionDto.ReactionType).Value;
        }

        public static List<UserReaction> ToEntities(this IEnumerable<UserReactionDto> userReactionDtos)
        {
            var entityList = userReactionDtos.Select(userReactionDto => userReactionDto.ToEntity()).ToList();
            return entityList;
        }
    }
}
