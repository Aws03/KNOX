using System.ComponentModel.DataAnnotations.Schema;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entities
{
    public sealed class Choice : AuditableEntity
    {
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; private set; }
        public Question Question { get; private set; } = null!;
        public string Text { get; private set; } = string.Empty;

        public string? ImageUrl { get; private set; }
        public bool IsCorrect { get; private set; }

        private Choice() { }

        private Choice(int questionId, string text, bool isCorrect,string? imageUrl)
        {
            QuestionId = questionId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            IsCorrect = isCorrect;
            ImageUrl = imageUrl;
        }

        public static Result<Choice> Create(int questionId, string text, bool isCorrect,string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return QuizErrors.ChoiceTextRequired;
            }
            var choice = new Choice(questionId, text, isCorrect,imageUrl);
            return choice;
        }
    }

}
