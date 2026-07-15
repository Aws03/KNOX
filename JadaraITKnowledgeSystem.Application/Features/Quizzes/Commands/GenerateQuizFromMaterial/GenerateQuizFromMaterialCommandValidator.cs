using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;

public sealed class GenerateQuizFromMaterialCommandValidator : AbstractValidator<GenerateQuizFromMaterialCommand>
{
    public GenerateQuizFromMaterialCommandValidator()
    {
        RuleFor(x => x.MaterialId)
            .GreaterThan(0)
            .WithMessage("Material ID must be greater than 0");

        RuleFor(x => x.WriterId)
            .GreaterThan(0)
            .WithMessage("Writer ID must be greater than 0");

        RuleFor(x => x.Options).NotNull()
            .WithMessage("Options are required");

        When(x => x.Options != null, () =>
        {
            RuleFor(x => x.Options.QuestionsPerQuiz)
                .InclusiveBetween(5, 20)
                .WithMessage("Questions per quiz must be between 5 and 20");

            RuleFor(x => x.Options.MaxQuizzes)
                .InclusiveBetween(1, 10)
                .WithMessage("Maximum quizzes must be between 1 and 10");

            RuleFor(x => x.Options.Difficulty)
                .Must(d => new[] { "Easy", "Medium", "Hard" }.Contains(d))
                .WithMessage("Difficulty must be Easy, Medium, or Hard");
        });
    }
}
