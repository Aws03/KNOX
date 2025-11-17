using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Domain.Quizzes.Errors;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;


namespace JadaraITKnowledgeSystem.Domain.Quizzes.Entites
{
    public class Question : AuditableEntity
    {
        //[Key]
        //public int QuestionId { get; private set; }
        [ForeignKey(nameof(Quiz))]
        public int QuizId { get; private set; }
        public Quiz Quiz { get; private set; }

        public QuestionType Type { get; private set; }
        public string Text { get; private set; }

        private readonly List<Choice> _choices = new();
        public IReadOnlyCollection<Choice> Choices => _choices.AsReadOnly();
        private Question() { }

        private Question(int quizId, QuestionType type, string text)
        {
            QuizId = quizId;
            Type = type;
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public void AddChoice(Choice choice)
        {
            if (choice == null) throw new ArgumentNullException(nameof(choice));
            _choices.Add(choice);
        }

        public static Result<Question> Create(int quizId, QuestionType type, string text)
        {
            //if (string.IsNullOrWhiteSpace(text))
            //{
            //    return 
            //}

            var question = new Question(quizId, type, text);
            return question;
        }

    }

}
