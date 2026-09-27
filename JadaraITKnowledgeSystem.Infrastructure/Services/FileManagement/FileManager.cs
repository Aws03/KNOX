using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;

public class FileManager : IFileManager
{
    private readonly IStorageService _storage;
    private readonly ILogger<FileManager> _logger;

    private static readonly HashSet<string> _allowedExtensions = new()
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".mp4",
        ".docx", ".pptx", ".pptm", ".xlsx", ".xlsm"
    };

    public FileManager(
        IStorageService storage,
        ILogger<FileManager> logger)
    {
        _storage = storage;
        _logger = logger;
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string extension,
        string folder,
        CancellationToken cancellationToken = default)
    {
        ValidateExtension(extension);
        ValidateStream(fileStream);

        string fileName = GenerateFileName(extension);

        _logger.LogInformation(
            "Uploading file {FileName} to folder {Folder}",
            fileName, folder);

        string fileUrl = await _storage.UploadAsync(
            fileStream,
            fileName,
            folder,
            cancellationToken);

        _logger.LogInformation(
            "Successfully uploaded file to {FileUrl}",
            fileUrl);

        return fileUrl;
    }

    public async Task<string> UpdateAsync(
        string? oldFileUrl,
        Stream newFileStream,
        string extension,
        string folder,
        CancellationToken cancellationToken = default)
    {
        ValidateExtension(extension);
        ValidateStream(newFileStream);

        _logger.LogInformation(
            "Updating file. Old: {OldUrl}, Folder: {Folder}",
            oldFileUrl, folder);

        // Upload new file first
        string newUrl = await UploadAsync(
            newFileStream,
            extension,
            folder,
            cancellationToken);

        // Best effort: the update has succeeded once the new file is stored.
        if (!string.IsNullOrWhiteSpace(oldFileUrl))
            await TryDeleteAsync(oldFileUrl);

        return newUrl;
    }

    public async Task<bool> DeleteAsync(
        string? fileUrl,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
        {
            _logger.LogWarning("Attempted to delete null or empty file URL");
            return false;
        }

        var (fileName, folder) = ExtractFileNameAndFolder(fileUrl);

        if (string.IsNullOrEmpty(fileName))
        {
            _logger.LogWarning("Could not extract filename from URL: {FileUrl}", fileUrl);
            return false;
        }

        _logger.LogInformation(
            "Deleting file {FileName} from folder {Folder}",
            fileName, folder);

        bool result = await _storage.DeleteAsync(fileName, folder, cancellationToken);

        if (result)
        {
            _logger.LogInformation("Successfully deleted file {FileUrl}", fileUrl);
        }
        else
        {
            _logger.LogWarning("File deletion returned false for {FileUrl}", fileUrl);
        }

        return result;
    }

    public async Task<string> MoveFromTempToPermanentAsync(
        string tempFileUrl,
        string permanentFolder,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tempFileUrl))
        {
            throw new ArgumentException("Temp file URL cannot be null or empty", nameof(tempFileUrl));
        }

        _logger.LogInformation(
            "Moving file from temp {TempUrl} to permanent folder {PermanentFolder}",
            tempFileUrl, permanentFolder);

        // Extract file info from temp URL
        var (tempFileName, tempFolder) = ExtractFileNameAndFolder(tempFileUrl);

        if (string.IsNullOrEmpty(tempFileName))
        {
            throw new ArgumentException("The temporary file URL is not a valid upload URL.", nameof(tempFileUrl));
        }

        using var tempStream = await DownloadFileFromStorageAsync(
            tempFileName,
            tempFolder,
            cancellationToken);

        // Upload to permanent location
        string extension = Path.GetExtension(tempFileName);
        var permanentUrl = await UploadAsync(
            tempStream,
            extension,
            permanentFolder,
            cancellationToken);

        // Best effort: anything left behind is swept up by TempFileCleanupJob.
        await TryDeleteAsync(tempFileUrl);

        _logger.LogInformation(
            "Successfully moved file from temp to permanent: {PermanentUrl}",
            permanentUrl);

        return permanentUrl;
    }

    public async Task<int> DeleteOldTempFilesAsync(
    TimeSpan olderThan,
    CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Starting cleanup of temp files older than {TimeSpan}",
            olderThan);

        // Get list of temp files from storage (recursive to include all subfolders)
        var tempFiles = await _storage.ListFilesAsync("temp", cancellationToken);

        var cutoffDate = DateTime.UtcNow - olderThan;
        var deletedCount = 0;

        foreach (var file in tempFiles)
        {
            if (file.IsDirectory || file.DateCreated >= cutoffDate)
                continue;

            try
            {
                // Build the file URL for deletion
                var fileUrl = _storage.GetFileUrl(file.ObjectName, file.Path);
                var deleted = await DeleteAsync(fileUrl, cancellationToken);

                if (deleted)
                {
                    deletedCount++;
                    _logger.LogDebug(
                        "Deleted temp file: {FileName} from {Path} (Created: {Created})",
                        file.ObjectName,
                        file.Path,
                        file.DateCreated);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete temp file {FileName} during cleanup",
                    file.ObjectName);
            }
        }

        _logger.LogInformation(
            "Temp file cleanup completed. Deleted {Count} files",
            deletedCount);

        return deletedCount;
    }

    // =============================
    // Private Helper Methods
    // =============================

    private async Task<Stream> DownloadFileFromStorageAsync(
        string fileName,
        string? folder,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading file from storage: {Folder}/{FileName}", folder, fileName);

        var stream = await _storage.DownloadAsync(fileName, folder, cancellationToken);

        if (stream == null)
        {
            _logger.LogWarning("File not found in storage: {Folder}/{FileName}", folder, fileName);
                throw new FileNotFoundException($"File '{fileName}' not found in folder '{folder}'.");
        }

        // Copy to a seekable MemoryStream so it works with ASP.NET File() results
        var memory = new MemoryStream();
        await stream.CopyToAsync(memory, cancellationToken);
        memory.Position = 0; // reset pointer

        return memory;
    }

    private async Task TryDeleteAsync(string fileUrl)
    {
        try
        {
            await DeleteAsync(fileUrl, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Best-effort delete of {FileUrl} failed", fileUrl);
        }
    }

    private static (string fileName, string? folder) ExtractFileNameAndFolder(string fileUrl)
    {
        if (!Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
            return (string.Empty, null);

        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0)
            return (string.Empty, null);

        var fileName = segments[^1];

        // Local storage URL format: /uploads/folder.../file
        if (segments.Length > 1 && segments[0].Equals("uploads", StringComparison.OrdinalIgnoreCase))
        {
            if (segments.Length > 2)
            {
                var folderSegments = segments[1..^1];
                return (fileName, string.Join("/", folderSegments));
            }

            return (fileName, null);
        }

        return (fileName, null);
    }


    private static string GenerateFileName(string extension)
    {
        return $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
    }

    private static void ValidateExtension(string extension)
    {
        if (string.IsNullOrWhiteSpace(extension))
        {
            throw new ArgumentException("File extension cannot be null or empty", nameof(extension));
        }

        var normalizedExtension = extension.ToLowerInvariant();
        if (!normalizedExtension.StartsWith("."))
        {
            normalizedExtension = "." + normalizedExtension;
        }

        if (!_allowedExtensions.Contains(normalizedExtension))
        {
            throw new ArgumentException(
                $"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }
    }

    private static void ValidateStream(Stream stream)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream), "File stream cannot be null");
        }

        if (!stream.CanRead)
        {
            throw new ArgumentException("File stream must be readable", nameof(stream));
        }

        if (stream.Length == 0)
        {
            throw new ArgumentException("File stream cannot be empty", nameof(stream));
        }
    }
}