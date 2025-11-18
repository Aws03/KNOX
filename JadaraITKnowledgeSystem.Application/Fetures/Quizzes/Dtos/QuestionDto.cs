using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos
{
    public sealed record QuestionDto
    {
        public int Id { get; init; }
        public int QuizId { get; init; }
        public QuestionType Type { get; init; } // 1: Single Choice, 2: Multiple Choice, 3: True/False, 4: Short Answer
        public string Text { get; init; } = String.Empty;
        public List<ChoiceDto> Choices { get; init; } = new();
    }
}
