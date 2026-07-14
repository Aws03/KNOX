using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.FileMangment;

public class FileManager : IFileManager
{
    private readonly IStorageService _storage;
    private readonly ILogger<FileManager> _logger;

    private static readonly HashSet<string> _allowedExtensions = new()
    {
        ".jpg", ".jpeg", ".png", ".webp", ".pdf", ".mp4",
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

        // Delete old file if exists (best effort - don't fail update if deletion fails)
        if (!string.IsNullOrWhiteSpace(oldFileUrl))
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await DeleteAsync(oldFileUrl, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Failed to delete old file {OldUrl} during update. New file uploaded successfully.",
                        oldFileUrl);
                }
            });
        }

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
            throw new InvalidOperationException($"Could not extract filename from temp URL: {tempFileUrl}");
        }

        // Download file from Bunny.net temp storage
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

        // Delete temp file (best effort - fire and forget)
        _ = Task.Run(async () =>
        {
            try
            {
                await DeleteAsync(tempFileUrl, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to delete temp file {TempUrl} after moving to permanent storage.",
                    tempFileUrl);
            }
        });

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
            // Skip directories
            if (file.IsDirectory)
                continue;

            
            if (file.DateCreated < cutoffDate)
            {
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
            else
            {
                _logger.LogWarning(
                    "Could not parse DateCreated for file {FileName}: {DateCreated}",
                    file.ObjectName,
                    file.DateCreated);
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

    private (string fileName, string? folder) ExtractFileNameAndFolder(string fileUrl)
    {
        var uri = new Uri(fileUrl);
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


    private string GenerateFileName(string extension)
    {
        return $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
    }

    private void ValidateExtension(string extension)
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
            throw new InvalidOperationException(
                $"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}");
        }
    }

    private void ValidateStream(Stream stream)
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