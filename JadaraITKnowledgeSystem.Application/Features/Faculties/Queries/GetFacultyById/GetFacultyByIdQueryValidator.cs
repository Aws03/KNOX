using System.Text;
using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultyById;

public sealed class GetFacultyByIdQueryValidator : AbstractValidator<GetFacultyByIdQuery>
{
    public GetFacultyByIdQueryValidator()
    {
        RuleFor(x => x.facultyId)
            .GreaterThan(0).WithMessage("Faculty Id must be greater than zero.");
    }
}
