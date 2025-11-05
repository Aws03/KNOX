using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class QuestionWithChoicesDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int Type { get; set; } // 1: Single Choice, 2: Multiple Choice, 3: True/False, 4: Short Answer
        public string Text { get; set; }

        public IEnumerable<ChoiceDto> Choices { get; set; } = Enumerable.Empty<ChoiceDto>();
    }
}
