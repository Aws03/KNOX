// TODO: Grade functionality is temporarily disabled.
// Universities may have different grading systems (A, A+, B, etc.)
// This needs to be redesigned to support flexible grading systems.

/*
using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AddGrade;

public sealed class AddGradeCommandValidator : AbstractValidator<AddGradeCommand>
{
    public AddGradeCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be a positive integer.");

        RuleFor(x => x.Grade)
            .InclusiveBetween(0, 100)
            .WithMessage("Grade must be between 0 and 100.");
    }
}
*/
