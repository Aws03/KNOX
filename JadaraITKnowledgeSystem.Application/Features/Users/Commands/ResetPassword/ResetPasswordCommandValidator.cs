using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Validators;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Otp)
            .NotEmpty().WithMessage("OTP is required")
            .Length(6).WithMessage("OTP must be 6 digits")
            .Matches(@"^\d{6}$").WithMessage("OTP must contain only digits");

        RuleFor(x => x.NewPassword).StrongPassword();
    }
}
