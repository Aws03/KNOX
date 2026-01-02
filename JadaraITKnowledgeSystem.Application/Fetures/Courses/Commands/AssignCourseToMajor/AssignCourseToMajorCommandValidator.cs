using FluentValidation;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.AssignCourseToMajor;

public sealed class AssignCourseToMajorCommandValidator : AbstractValidator<AssignCourseToMajorCommand>
{
    public AssignCourseToMajorCommandValidator()
    {
        RuleFor(x => x.CourseId)
            .GreaterThan(0)
            .WithMessage("CourseId must be a positive integer.");

        RuleFor(x => x.MajorId)
            .GreaterThan(0)
            .WithMessage("MajorId must be a positive integer.");

        RuleFor(x => x.RequirementType)
            .IsInEnum()
            .WithMessage("RequirementType must be a valid value.");

        RuleFor(x => x.RequirementNature)
            .IsInEnum()
            .WithMessage("RequirementNature must be a valid value.");
    }
}
