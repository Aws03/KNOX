using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Events
{
    public sealed class QuizCreatedEvent : DomainEvent
    {
        public int QuizId { get; set; }
    }
}
