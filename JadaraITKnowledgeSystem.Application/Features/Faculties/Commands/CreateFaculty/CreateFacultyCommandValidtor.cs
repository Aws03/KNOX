using FluentValidation;


namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.CreateFaculty;

public sealed class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
{
    public CreateFacultyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Faculty name is required.")
            .MaximumLength(100).WithMessage("Faculty name must not exceed 100 characters.");

        RuleFor(x => x.UniversityId)
            .NotNull().WithMessage("University ID is required.");
    }
}
