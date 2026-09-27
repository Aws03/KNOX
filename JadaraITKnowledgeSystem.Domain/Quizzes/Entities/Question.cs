using System.ComponentModel.DataAnnotations.Schema;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entities
{
    public class Question : AuditableEntity
    {
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; } = null!;

        public QuestionType Type { get; private set; }
        public string Text { get; private set; } = string.Empty;
        public string? ImageUrl { get; private set; }

        private readonly List<Choice> _choices = new();
        public IReadOnlyCollection<Choice> Choices => _choices.AsReadOnly();
        private Question() { }

        private Question(int quizId, QuestionType type, string text,string? imageUrl)
        {
            QuizId = quizId;
            Type = type;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            ImageUrl = imageUrl;
        }

        public void AddChoice(Choice choice)
        {
            if (choice == null) throw new ArgumentNullException(nameof(choice));
            _choices.Add(choice);
        }

        public static Result<Question> Create(int quizId, QuestionType type, string text, string? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return QuizErrors.QuestionTextRequired;

            return new Question(quizId, type, text, imageUrl);
        }

    }

}
