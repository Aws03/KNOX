using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.UpdateMajor;

public sealed class UpdateMajorCommandValidator : AbstractValidator<UpdateMajorCommand>
{
    public UpdateMajorCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Major ID must be greater than 0.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Major name is required.")
            .MaximumLength(100).WithMessage("Major name must not exceed 100 characters.");

        RuleFor(x => x.FacultyId)
            .GreaterThan(0).WithMessage("Faculty ID must be greater than 0.");
    }
}
