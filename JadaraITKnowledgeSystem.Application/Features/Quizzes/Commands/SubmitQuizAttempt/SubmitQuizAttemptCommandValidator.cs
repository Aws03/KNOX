using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandValidator : AbstractValidator<SubmitQuizAttemptCommand>
{
    public SubmitQuizAttemptCommandValidator()
    {
        RuleFor(x => x.QuizId)
            .GreaterThan(0).WithMessage("A valid quiz id is required.");

        RuleFor(x => x.Score)
            .InclusiveBetween(0, 100).WithMessage("Score must be between 0 and 100.");
    }
}
