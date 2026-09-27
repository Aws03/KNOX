using System.Text.RegularExpressions;
using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

/// <summary>
/// File uploads. A thin pass-through to the Application's IFileManager port; there is no
/// business logic to route through a command.
/// </summary>
[ApiController]
[Route("api/files")]
[Authorize]
public sealed partial class FilesController(IFileManager fileManager, TimeProvider timeProvider) : ControllerBase
{
    private const string QuizImageCategory = "quiz-question";

    /// <summary>Uploads a quiz image to temporary storage; creating the quiz moves it to permanent storage.</summary>
    [HttpPost("upload/temporary")]
    [RequestSizeLimit(10_000_000)]
    [ProducesResponseType<UploadedFileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> UploadTemporaryFile(IFormFile file, CancellationToken cancellationToken) =>
        UploadAsync(file, QuizImageCategory, "temp", cancellationToken);

    /// <summary>Uploads straight to permanent storage (course materials).</summary>
    [HttpPost("upload/permanent")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<UploadedFileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> UploadPermanentFile(IFormFile file, [FromForm] string fileCategory, CancellationToken cancellationToken)
    {
        // The category becomes a folder name on disk, so it must be a single plain segment.
        if (string.IsNullOrWhiteSpace(fileCategory) || !CategoryPattern().IsMatch(fileCategory))
            return Task.FromResult<IActionResult>(Problem(detail: "Invalid file category.", statusCode: StatusCodes.Status400BadRequest));

        return UploadAsync(file, fileCategory, $"permanent/{fileCategory}", cancellationToken);
    }

    [HttpDelete]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileUrl, CancellationToken cancellationToken) =>
        await fileManager.DeleteAsync(fileUrl, cancellationToken)
            ? NoContent()
            : Problem(detail: "File not found.", statusCode: StatusCodes.Status404NotFound);

    private async Task<IActionResult> UploadAsync(IFormFile? file, string category, string folder, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Problem(detail: "No file uploaded.", statusCode: StatusCodes.Status400BadRequest);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions(category).Contains(extension))
            return Problem(detail: $"File type '{extension}' is not allowed here.", statusCode: StatusCodes.Status400BadRequest);

        await using var stream = file.OpenReadStream();
        var fileUrl = await fileManager.UploadAsync(stream, extension, folder, cancellationToken);

        return Ok(new UploadedFileResponse(fileUrl, file.FileName, file.Length, timeProvider.GetUtcNow()));
    }

    private static HashSet<string> AllowedExtensions(string category) => category switch
    {
        "quiz-question" or "quiz-choice" => [".jpg", ".jpeg", ".png", ".gif"],
        "material" => [".pdf", ".jpg", ".jpeg", ".png", ".docx", ".pptx", ".mp4"],
        _ => [".jpg", ".jpeg", ".png"]
    };

    [GeneratedRegex("^[A-Za-z0-9_-]{1,50}$")]
    private static partial Regex CategoryPattern();
}
