using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultyById
{
    public sealed class GetQeuryByIdQueryValidator : AbstractValidator<GetFacultyByIdQuery>
    {
        public GetQeuryByIdQueryValidator()
        {
            RuleFor(x => x.facultyId)
                .GreaterThan(0).WithMessage("Faculty Id must be greater than zero.");
        }
    }
}
