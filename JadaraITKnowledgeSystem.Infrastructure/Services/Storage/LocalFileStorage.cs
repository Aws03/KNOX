using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

/// <summary>
/// Stores files on local disk under wwwroot/uploads, served back via the
/// static files middleware at "{baseUrl}/uploads/...". Intended for local/
/// development use in place of a CDN-backed provider.
/// </summary>
public class LocalFileStorage : IStorageService
{
    private const string UploadsFolderName = "uploads";

    private readonly string _uploadsRootPath;
    private readonly string _baseUrl;

    public LocalFileStorage(IHostEnvironment env, IConfiguration configuration)
    {
        _uploadsRootPath = Path.Combine(env.ContentRootPath, "wwwroot", UploadsFolderName);
        Directory.CreateDirectory(_uploadsRootPath);

        _baseUrl = configuration["Storage:BaseUrl"]?.TrimEnd('/')
            ?? throw new ArgumentNullException("Storage:BaseUrl", "Storage BaseUrl is not configured.");
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var folderPath = BuildFolderPath(path);
        Directory.CreateDirectory(folderPath);

        var fullFilePath = Path.Combine(folderPath, fileName);

        if (fileStream.CanSeek)
            fileStream.Position = 0;

        using var output = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(output, cancellationToken);

        return GetFileUrl(fileName, path);
    }

    public Task<bool> DeleteAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine(BuildFolderPath(path), fileName);

        if (!File.Exists(fullFilePath))
            return Task.FromResult(false);

        File.Delete(fullFilePath);
        return Task.FromResult(true);
    }

    public Task<List<StorageFileInfo>> ListFilesAsync(
        string? path,
        CancellationToken cancellationToken = default)
    {
        var folderPath = BuildFolderPath(path);
        var result = new List<StorageFileInfo>();

        if (!Directory.Exists(folderPath))
            return Task.FromResult(result);

        foreach (var filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
        {
            var relativeDir = Path.GetDirectoryName(Path.GetRelativePath(_uploadsRootPath, filePath)) ?? string.Empty;

            result.Add(new StorageFileInfo
            {
                ObjectName = Path.GetFileName(filePath),
                Path = relativeDir.Replace(Path.DirectorySeparatorChar, '/'),
                IsDirectory = false,
                DateCreated = File.GetCreationTimeUtc(filePath),
                LastChanged = File.GetLastWriteTimeUtc(filePath),
                Length = new FileInfo(filePath).Length
            });
        }

        return Task.FromResult(result);
    }

    public async Task<Stream?> DownloadAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullFilePath = Path.Combine(BuildFolderPath(path), fileName);

        if (!File.Exists(fullFilePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(fullFilePath, cancellationToken);
        return new MemoryStream(bytes);
    }

    public string GetFileUrl(string fileName, string? path = null)
    {
        return string.IsNullOrEmpty(path)
            ? $"{_baseUrl}/{UploadsFolderName}/{fileName}"
            : $"{_baseUrl}/{UploadsFolderName}/{path}/{fileName}";
    }

    private string BuildFolderPath(string? folder)
        => string.IsNullOrEmpty(folder)
            ? _uploadsRootPath
            : Path.Combine(_uploadsRootPath, folder.Replace('/', Path.DirectorySeparatorChar));
}
