using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.EnrollCourse;

public sealed class EnrollCourseCommandValidator : AbstractValidator<EnrollCourseCommand>
{
    public EnrollCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be a positive integer.");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters.")
            .When(x => x.Notes != null);
    }
}
