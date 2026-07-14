using FluentValidation;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateProfilePicture;

public sealed class UpdateProfilePictureCommandValidator : AbstractValidator<UpdateProfilePictureCommand>
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

    public UpdateProfilePictureCommandValidator()
    {
        RuleFor(x => x.ImageStream)
            .NotNull()
            .WithMessage("Image stream is required.");

        RuleFor(x => x.ImageStream.Length)
            .LessThanOrEqualTo(MaxFileSize)
            .When(x => x.ImageStream != null && x.ImageStream.CanSeek)
            .WithMessage($"Image file size must not exceed {MaxFileSize / (1024 * 1024)} MB.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required.")
            .Must(fileName => AllowedExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .WithMessage($"Only image files ({string.Join(", ", AllowedExtensions)}) are allowed.");
    }
}
