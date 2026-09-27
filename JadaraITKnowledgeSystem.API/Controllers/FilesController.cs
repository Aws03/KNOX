using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
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
public sealed class FilesController(ISender sender, IFileManager fileManager, TimeProvider timeProvider) : ApiControllerBase(sender)
{
    private static readonly HashSet<string> QuizImageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    /// <summary>Uploads a quiz image to temporary storage; creating the quiz moves it to permanent storage.</summary>
    [HttpPost("upload/temporary")]
    [RequestSizeLimit(10_000_000)]
    [ProducesResponseType<UploadedFileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadTemporaryFile(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return Problem(detail: "No file uploaded.", statusCode: StatusCodes.Status400BadRequest);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!QuizImageExtensions.Contains(extension))
            return Problem(detail: $"File type '{extension}' is not allowed here.", statusCode: StatusCodes.Status400BadRequest);

        await using var stream = file.OpenReadStream();
        var fileUrl = await fileManager.UploadAsync(stream, extension, "temp/quiz-images", cancellationToken);

        return Ok(new UploadedFileResponse(fileUrl, file.FileName, file.Length, timeProvider.GetUtcNow()));
    }

    /// <summary>
    /// Starts a course-material upload. The client PUTs the file to the returned URL (straight to object
    /// storage, so large videos never pass through the API), then creates the material with the returned key.
    /// </summary>
    [HttpPost("material-uploads")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<DirectUpload>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult CreateMaterialUpload([FromBody] CreateMaterialUploadRequest request) =>
        OkOrProblem(fileManager.CreateMaterialUpload(request.FileName, request.Size));
}
