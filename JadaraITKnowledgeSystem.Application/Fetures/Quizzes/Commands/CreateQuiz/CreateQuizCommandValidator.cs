using FluentValidation;
using System.Linq;

namespace JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{   
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Quiz title is required.")
            .MaximumLength(200).WithMessage("Quiz title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Quiz description must not exceed 1000 characters.");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Tag cannot be empty.")
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count() <= 10)
            .WithMessage("A maximum of 10 tags is allowed per quiz.");
    }
}
