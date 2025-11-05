using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class UserReaction : AuditableEntity
    {
        //public int UserReactionId { get; private set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; }
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; }
        public bool IsLike { get; private set; }

        private UserReaction() { }

        public UserReaction(int userId, int quizId, bool isLike)
        {
            UserId = userId;
            QuizId = quizId;
            IsLike = isLike;
        }
    }

}
