using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseInfo;

public sealed class CreateCourseInfoCommandValidator : AbstractValidator<CreateCourseInfoCommand>
{
    public CreateCourseInfoCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be greater than 0");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.DemonstrationVideoUrl)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.DemonstrationVideoUrl))
            .WithMessage("Demonstration video URL cannot exceed 1000 characters")
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.DemonstrationVideoUrl))
            .WithMessage("Demonstration video URL must be a valid URL");

        RuleFor(x => x.DemonstrationVideoTitle)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.DemonstrationVideoTitle))
            .WithMessage("Demonstration video title cannot exceed 200 characters");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}
