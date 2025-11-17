using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz
{
    public sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
    {   
        public CreateQuizCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Quiz title is required.")
                .MaximumLength(200).WithMessage("Quiz title must not exceed 200 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Quiz description must not exceed 1000 characters.");
            //RuleFor(x => x.Questions)
            //    .NotEmpty().WithMessage("At least one question is required.")
            //    .Must(questions => questions != null && questions.Count > 0).WithMessage("At least one question is required.");
            //RuleForEach(x => x.Questions).SetValidator(new CreateQuestionCommandValidator());
        }

    }
}
