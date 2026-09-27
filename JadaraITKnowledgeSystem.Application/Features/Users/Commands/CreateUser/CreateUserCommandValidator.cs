using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Validators;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;

// Mirrors the Identity password policy configured in Program.cs so bad input is
// rejected before a domain user row is written (and has to be compensated).
public sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(200).WithMessage("Full name must not exceed 200 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(254).WithMessage("Email must not exceed 254 characters.");

        RuleFor(x => x.MajorId)
            .GreaterThan(0).WithMessage("MajorId must be a positive integer.");

        When(x => x.Password is not null, () =>
        {
            RuleFor(x => x.Password!).StrongPassword();
        });
    }
}
