using System.Text;
using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateFolder
{
    public class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
    {
        public CreateFolderCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Folder name is required.")
                .MaximumLength(200)
                .WithMessage("Folder name must not exceed 200 characters.");

            RuleFor(x => x.CourseId)
                .GreaterThan(0)
                .WithMessage("Course ID must be a positive integer.");

            RuleFor(x => x.ParentFolderId)
                .GreaterThan(0)
                .WithMessage("Parent folder ID must be a positive integer.")
                .When(x => x.ParentFolderId.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters.");
        }
    }
}
