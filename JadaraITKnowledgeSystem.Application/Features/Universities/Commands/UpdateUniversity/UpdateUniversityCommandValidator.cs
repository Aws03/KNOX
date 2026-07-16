using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Commands.UpdateUniversity;

public sealed class UpdateUniversityCommandValidator : AbstractValidator<UpdateUniversityCommand>
{
    public UpdateUniversityCommandValidator()
    {
        RuleFor(u => u.Id)
            .GreaterThan(0).WithMessage("University ID must be greater than 0.");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("University Name is required.")
            .MaximumLength(100).WithMessage("University Name must not exceed 100 characters.");
    }
}
