using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos
{
    public sealed record ChoiceDto
    {
        public int Id { get; init; }
        public int QuestionId { get; init; }
        public string Text { get; init; } = String.Empty;
        public bool IsCorrect { get; init; }
    }
}
