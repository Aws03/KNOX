using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.CreateMajor;

public sealed class CreateMajorCommandValidator : AbstractValidator<CreateMajorCommand>
{
    public CreateMajorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Major name is required.")
            .MaximumLength(100).WithMessage("Major name must not exceed 100 characters.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0).WithMessage("Faculty ID must be greater than 0.");
    }
}
