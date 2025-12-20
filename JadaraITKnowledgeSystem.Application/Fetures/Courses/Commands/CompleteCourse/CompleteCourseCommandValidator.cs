using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CompleteCourse;

public sealed class CompleteCourseCommandValidator : AbstractValidator<CompleteCourseCommand>
{
    public CompleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be a positive integer.");

        // TODO: Grade functionality is temporarily disabled.
        // Universities may have different grading systems (A, A+, B, etc.)
        // RuleFor(x => x.Grade)
        //     .InclusiveBetween(0, 100)
        //     .WithMessage("Grade must be between 0 and 100.")
        //     .When(x => x.Grade.HasValue);
    }
}
