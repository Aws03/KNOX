using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entites
{
    public sealed class UserReaction : AuditableEntity
    {
        //public int UserReactionId { get; private set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; }
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; }
        public ReactionType ReactionType { get; private set; }

        private UserReaction() { }

        private UserReaction(int userId, int quizId, ReactionType reactionType)
        {
            UserId = userId;
            QuizId = quizId;
            ReactionType = reactionType;
        }

        public static Result<UserReaction> Create(int userId, int quizId, ReactionType reactionType)
        {
            return new UserReaction(userId, quizId, reactionType);
        }
    }

}
