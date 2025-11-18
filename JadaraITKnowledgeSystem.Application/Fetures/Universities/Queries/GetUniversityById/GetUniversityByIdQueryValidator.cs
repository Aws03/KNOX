using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversityById
{
    public sealed class GetUniversityByIdQueryValidator
        : AbstractValidator<GetUniversityByIdQuery>
    {
        public GetUniversityByIdQueryValidator()
        {
            RuleFor(x => x.UniversityId)
                .GreaterThan(0)
                .WithMessage("University ID must be greater than 0.");
        }
    }
}
