using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Commands.CreateMajor;

public sealed class CreateMajorCommandValidator : AbstractValidator<CreateMajorCommand>
{
    public CreateMajorCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Faculty name is required.")
            .MaximumLength(100).WithMessage("Faculty name must not exceed 100 characters.");

        RuleFor(x => x.FacultyId)
            .NotNull().WithMessage("University ID is required.");
    }
}
