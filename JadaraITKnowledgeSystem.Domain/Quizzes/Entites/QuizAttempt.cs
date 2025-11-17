using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
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
    public sealed class QuizAttempt : AuditableEntity
    {
        //[Key]
        //public int QuizAttemptId { get; private set; }

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; }

        public decimal Score { get; private set; }
        public DateTime AttemptDate { get; private set; }

        private QuizAttempt(int quizId, int userId, decimal score)
        {
            SetScore(score);
            QuizId = quizId;
            UserId = userId;
            AttemptDate = DateTime.UtcNow;
        }

        public static Result<QuizAttempt> Create(int quizId, int userId, decimal score)
        {
            return new QuizAttempt(quizId, userId, score);
        }

        private void SetScore(decimal score)
        {
            if (score < 0 || score > 100) throw new ArgumentException("Score must be between 0 and 100.");
            Score = score;
        }

        // Update result if needed (only last result counts)
        public void UpdateScore(decimal newScore)
        {
            SetScore(newScore);
            AttemptDate = DateTime.UtcNow;
        }
    }

}
