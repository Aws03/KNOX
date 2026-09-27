using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName))
            .WithMessage("Full name cannot exceed 200 characters.");

        RuleFor(x => x.MajorId)
            .GreaterThan(0)
            .When(x => x.MajorId.HasValue)
            .WithMessage("Major ID must be a positive integer.");

        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.FullName) || x.MajorId.HasValue)
            .WithMessage("At least one field (FullName or MajorId) must be provided.")
            .OverridePropertyName("FullName"); // a named field, so the error is not keyed by ""
    }
}
