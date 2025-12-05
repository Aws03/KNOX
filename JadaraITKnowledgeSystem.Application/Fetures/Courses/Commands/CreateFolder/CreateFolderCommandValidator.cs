using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateFolder
{
    public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
    {
        public CreateFolderCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Folder name is required.");

            RuleFor(x => x.CourseId)
                .NotEmpty()
                .WithMessage("Course ID is required.");

            RuleFor(x => x.ParentFolderId)
                .NotEmpty()
                .WithMessage("Parent folder ID is required.")
                .When(x => x.ParentFolderId.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");
        }
    }
}
