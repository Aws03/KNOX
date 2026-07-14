using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AddCourseResource;

public sealed class AddCourseResourceCommandValidator : AbstractValidator<AddCourseResourceCommand>
{
    public AddCourseResourceCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be greater than 0");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("URL is required")
            .MaximumLength(1000)
            .WithMessage("URL cannot exceed 1000 characters")
            .Must(BeAValidUrl)
            .WithMessage("URL must be a valid URL");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 1000 characters");

        RuleFor(x => x.DemonstrationVideoUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.DemonstrationVideoUrl))
            .WithMessage("Demonstration video URL cannot exceed 1000 characters")
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.DemonstrationVideoUrl))
            .WithMessage("Demonstration video URL must be a valid URL");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
