using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Queries;

namespace JadaraITKnowledgeSystem.Application.Common.Validators;

/// <summary>Base validator for paginated queries; derive from it to add query-specific rules.</summary>
public abstract class PaginatedRequestValidator<TRequest> : AbstractValidator<TRequest>
    where TRequest : IPaginatedRequest
{
    public const int MaxPageSize = 50;

    protected PaginatedRequestValidator()
    {
        RuleFor(q => q.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than 0.");

        RuleFor(q => q.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than 0.")
            .LessThanOrEqualTo(MaxPageSize).WithMessage($"Page size must not exceed {MaxPageSize}.");
    }
}
