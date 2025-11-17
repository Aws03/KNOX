using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Commands.CreateUniversity
{
    public sealed class CreateUniversityCommandValidator : AbstractValidator<CreateUniversityCommand>
    {
        public CreateUniversityCommandValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("University Name is required.")
                .MaximumLength(100).WithMessage("University Name must not exceed 100 characters.");
        }
    }
    
}
