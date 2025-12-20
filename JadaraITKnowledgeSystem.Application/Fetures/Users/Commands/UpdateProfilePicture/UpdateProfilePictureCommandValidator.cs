using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.UpdateProfilePicture;

public sealed class UpdateProfilePictureCommandValidator : AbstractValidator<UpdateProfilePictureCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UpdateProfilePictureCommandValidator()
    {
        RuleFor(x => x.Image)
            .NotNull()
            .WithMessage("Image file is required.");

        RuleFor(x => x.Image.Length)
            .LessThanOrEqualTo(MaxFileSize)
            .When(x => x.Image != null)
            .WithMessage($"Image file size must not exceed {MaxFileSize / (1024 * 1024)} MB.");

        RuleFor(x => x.Image.FileName)
            .Must(fileName => AllowedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .When(x => x.Image != null)
            .WithMessage($"Only image files ({string.Join(", ", AllowedExtensions)}) are allowed.");
    }
}
