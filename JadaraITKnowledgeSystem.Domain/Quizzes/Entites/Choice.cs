using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entites
{
    public sealed class Choice : AuditableEntity
    {
        //[Key]
        //public int ChoiceId { get; private set; }
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; private set; }
        public Question Question { get; private set; }
        public string Text { get; private set; }

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
