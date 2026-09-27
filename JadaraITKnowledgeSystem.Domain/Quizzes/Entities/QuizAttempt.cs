using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entities
{
    public sealed class QuizAttempt : AuditableEntity
    {

        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; } = null!;

        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;

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
