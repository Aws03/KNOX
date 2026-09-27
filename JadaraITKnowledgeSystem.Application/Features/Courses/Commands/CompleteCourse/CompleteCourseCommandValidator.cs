using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CompleteCourse;

public sealed class CompleteCourseCommandValidator : AbstractValidator<CompleteCourseCommand>
{
    public CompleteCourseCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be a positive integer.");
    }
}
