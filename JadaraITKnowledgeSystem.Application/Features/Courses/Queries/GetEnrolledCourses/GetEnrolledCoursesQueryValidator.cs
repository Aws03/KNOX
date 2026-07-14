using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Queries.GetEnrolledCourses;

public sealed class GetEnrolledCoursesQueryValidator : AbstractValidator<GetEnrolledCoursesQuery>
{
    public GetEnrolledCoursesQueryValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(50).WithMessage("Page size must not exceed 50.");
    }
}
