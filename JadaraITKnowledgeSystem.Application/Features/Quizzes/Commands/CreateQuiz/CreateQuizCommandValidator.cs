using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;

public sealed class CreateQuizCommandValidator : AbstractValidator<CreateQuizCommand>
{
    public CreateQuizCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Quiz title is required.")
            .MaximumLength(200).WithMessage("Quiz title must not exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Quiz description must not exceed 1000 characters.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be a positive integer.");

        RuleFor(x => x.WriterId)
            .GreaterThan(0).WithMessage("WriterId must be a positive integer.");

        RuleFor(x => x.Questions)
            .NotNull().WithMessage("Questions are required.");

        RuleForEach(x => x.Questions).ChildRules(question =>
        {
            question.RuleFor(q => q.Text)
                .NotEmpty().WithMessage("Question text is required.");

            question.RuleFor(q => q.Type)
                .IsInEnum().WithMessage("Question type is invalid.");

            question.RuleForEach(q => q.Choices).ChildRules(choice =>
            {
                choice.RuleFor(c => c.Text)
                    .NotEmpty().WithMessage("Choice text is required.")
                    .MaximumLength(500).WithMessage("Choice text must not exceed 500 characters.");
            });
        });

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Tag cannot be empty.")
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count() <= 10)
            .WithMessage("A maximum of 10 tags is allowed per quiz.");
    }
}
