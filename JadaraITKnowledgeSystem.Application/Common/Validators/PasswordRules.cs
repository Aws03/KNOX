using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Common.Validators;

/// <summary>
/// The one password policy, mirroring the ASP.NET Identity options configured in the API
/// (length 8, upper, lower, digit; symbols allowed but not required).
/// </summary>
public static class PasswordRules
{
    public const int MinimumLength = 8;

    public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule) =>
        rule
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(MinimumLength).WithMessage($"Password must be at least {MinimumLength} characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.");
}
