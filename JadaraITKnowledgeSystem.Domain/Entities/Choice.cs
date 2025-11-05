using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class Choice : AuditableEntity
    {
        //[Key]
        //public int ChoiceId { get; private set; }
        [ForeignKey(nameof(Question))]
        public int QuestionId { get; private set; }
        public Question Question { get; private set; }
        public string Text { get; private set; }
        public bool IsCorrect { get; private set; }

        private Choice() { }

        public Choice(int questionId, string text, bool isCorrect)
        {
            QuestionId = questionId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            IsCorrect = isCorrect;
        }
    }

}
