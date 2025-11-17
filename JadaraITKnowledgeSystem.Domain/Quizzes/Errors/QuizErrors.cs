using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Quizzes.Errors
{
    public static class QuizErrors
    {
        public static Error TitleRequired =>
            Error.Validation("Quiz_Title_Required", "Quiz title is required.");

        public static Error QuestionRequired =>
            Error.Validation("Question_Required", "Quiz question can not be null.");

        public static Error ConflictReaction =>
            Error.Conflict("Cannot_Dublicate_Reaction", "Can not add the same reaction on the same quiz.");

        public static Error ChoiceTextRequired =>
            Error.Validation("Choice_Text_Required", "Choice text is required.");
    }
}
