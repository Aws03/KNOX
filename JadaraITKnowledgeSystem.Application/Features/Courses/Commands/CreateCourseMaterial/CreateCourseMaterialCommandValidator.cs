using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourseMaterial;

public sealed class CreateCourseMaterialCommandValidator : AbstractValidator<CreateCourseMaterialCommand>
{
    public CreateCourseMaterialCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

        RuleFor(x => x.UploadKey)
            .NotEmpty().WithMessage("Upload the file first; the upload key is required.")
            .MaximumLength(200);

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("CourseId must be greater than zero.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleForEach(x => x.Tags)
            .NotEmpty().WithMessage("Tag cannot be empty.")
            .MaximumLength(50).WithMessage("Tag cannot exceed 50 characters.");

        RuleFor(x => x.Tags)
            .Must(tags => tags == null || tags.Count() <= 10)
            .WithMessage("A maximum of 10 tags is allowed per material.");
    }
}
