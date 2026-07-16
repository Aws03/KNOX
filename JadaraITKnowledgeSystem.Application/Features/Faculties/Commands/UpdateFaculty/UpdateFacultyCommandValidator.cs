using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.UpdateFaculty;

public sealed class UpdateFacultyCommandValidator : AbstractValidator<UpdateFacultyCommand>
{
    public UpdateFacultyCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Faculty ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Faculty name is required.")
            .MaximumLength(100).WithMessage("Faculty name must not exceed 100 characters.");

        RuleFor(x => x.UniversityId)
            .GreaterThan(0).WithMessage("University ID must be greater than 0.");
    }
}
