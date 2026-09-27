using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entities
{
    public sealed class UserReaction : AuditableEntity
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; } = null!;
        public ReactionType ReactionType { get; private set; }

        private UserReaction() { }

        private UserReaction(int userId, int quizId, ReactionType reactionType)
        {
            UserId = userId;
            QuizId = quizId;
            ReactionType = reactionType;
        }

        public void ChangeType(ReactionType reactionType) => ReactionType = reactionType;

        public static Result<UserReaction> Create(int userId, int quizId, ReactionType reactionType)
        {
            return new UserReaction(userId, quizId, reactionType);
        }
    }

}
