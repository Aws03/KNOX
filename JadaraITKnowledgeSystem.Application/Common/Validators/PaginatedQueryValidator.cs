using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Common.Validators
{
    public class PaginatedQueryValidator<TQuery, TResponse> : AbstractValidator<TQuery>
    where TQuery : PaginatedQuery<TResponse>
    {
        public PaginatedQueryValidator()
        {
            RuleFor(q => q.PageNumber)
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(q => q.PageSize)
                .GreaterThan(0).WithMessage("Page size must be greater than 0.")
                .LessThanOrEqualTo(50).WithMessage("Page size must not exceed 50");
        }
    }
}
