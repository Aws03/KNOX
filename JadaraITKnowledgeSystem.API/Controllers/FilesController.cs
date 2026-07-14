using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[ApiController]
[Route("api/files")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IFileManager _fileManager;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileManager fileManager, ILogger<FilesController> logger)
    {
        _fileManager = fileManager;
        _logger = logger;
    }

    [HttpPost("upload/temporary")]
    [RequestSizeLimit(10_000_000)] // 10MB
    public async Task<IActionResult> UploadTemporaryFile(
        IFormFile file,
        //[FromForm] string fileCategory, // "quiz-question", "quiz-choice", "material", etc.
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        var allowedExtensions = GetAllowedExtensions("quiz-question");
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
            return BadRequest($"File type {extension} not allowed for this type of files");

        using var stream = file.OpenReadStream();

        // Upload to temporary folder
        var fileUrl = await _fileManager.UploadAsync(
            stream,
            extension,
            folder: $"temp",
            cancellationToken
        );

        return Ok(new 
        {
            FileUrl = fileUrl,
            FileName = file.FileName,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        });
    }

    [HttpPost("upload/permanent")]
    public async Task<IActionResult> UploadPermanentFile(
        IFormFile file,
        [FromForm] string fileCategory,
        CancellationToken cancellationToken)
    {
        // Similar to above but uploads to permanent folder directly
        // Used for materials that are uploaded one at a time

        using var stream = file.OpenReadStream();

        var fileUrl = await _fileManager.UploadAsync(
            stream,
            Path.GetExtension(file.FileName).ToLowerInvariant(),
            folder: $"permanent/{fileCategory}", // "materials", "profiles", etc.
            cancellationToken
        );

        return Ok(new 
        {
            FileUrl = fileUrl,
            FileName = file.FileName,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        });
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteFile(
        [FromQuery] string fileUrl,
        CancellationToken cancellationToken)
    {
        var result = await _fileManager.DeleteAsync(fileUrl, cancellationToken);

        if (!result)
            return NotFound("File not found");

        return NoContent();
    }

    private static HashSet<string> GetAllowedExtensions(string category) => category switch
    {
        "quiz-question" or "quiz-choice" => new() { ".jpg", ".jpeg", ".png", ".gif" },
        "material" => new() { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".pptx", ".mp4" },
        _ => new() { ".jpg", ".jpeg", ".png" }
    };
}

