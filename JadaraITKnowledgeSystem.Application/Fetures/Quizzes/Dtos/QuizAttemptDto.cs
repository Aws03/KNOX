using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos
{
    public sealed record QuizAttemptDto
    {
        public int Id { get; init; }
        public int QuizId { get; init; }
        public int UserId { get; init; }
        public decimal Score { get; init; }
        public DateTime AttemptDate { get; init; }
    }
}
