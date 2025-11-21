using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultiesByUniversityId;

public sealed class GetFacultiesByUniversityIdQueryValidator : AbstractValidator<GetFacultiesByUniversityIdQuery>
{
    public GetFacultiesByUniversityIdQueryValidator()
    {
        RuleFor(x => x.UniversityId)
            .GreaterThan(0).WithMessage("UniversityId must be greater than zero.");
    }
}
