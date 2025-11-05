using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
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

        public Question(int quizId, QuestionType type, string text)
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
    }

}
