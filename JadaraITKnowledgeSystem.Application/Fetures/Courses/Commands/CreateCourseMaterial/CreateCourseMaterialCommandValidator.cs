using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial
{
    public sealed class CreateCourseMaterialCommandValidator : AbstractValidator<CreateCourseMaterialCommand>
    {
        public CreateCourseMaterialCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(250).WithMessage("Title must not exceed 250 characters.");

            RuleFor(x => x.ContemtUrl)
                .NotEmpty().WithMessage("Content URL is required.")
                .MaximumLength(500).WithMessage("Content URL must not exceed 500 characters.")
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                             && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Content URL must be a valid HTTP/HTTPS URL.");

            RuleFor(x => x.CourseId)
                .GreaterThan(0).WithMessage("CourseId must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        }
    }
}
