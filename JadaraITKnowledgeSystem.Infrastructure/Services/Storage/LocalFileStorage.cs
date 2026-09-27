using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

/// <summary>
/// Stores files on local disk (Storage:RootPath, default wwwroot/uploads), served by the API's
/// static-file mapping at "{BaseUrl}/uploads/...".
/// Folder and file names can originate from request data, so every path is resolved
/// and checked to stay inside the uploads root before touching the disk.
/// </summary>
public class LocalFileStorage : IStorageService
{
    private readonly string _uploadsRootPath;
    private readonly string _baseUrl;

    public LocalFileStorage(IHostEnvironment env, IOptions<StorageOptions> options)
    {
        _uploadsRootPath = options.Value.ResolveRootPath(env.ContentRootPath);
        Directory.CreateDirectory(_uploadsRootPath);
        _baseUrl = options.Value.BaseUrl.TrimEnd('/') + StorageOptions.RequestPath;
    }

    public async Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullFilePath = ResolveFilePath(fileName, path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullFilePath)!);

        if (fileStream.CanSeek)
            fileStream.Position = 0;

        await using var output = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(output, cancellationToken);

        return GetFileUrl(fileName, path);
    }

    public Task<bool> DeleteAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default)
    {
        var fullFilePath = ResolveFilePath(fileName, path);

        if (!File.Exists(fullFilePath))
            return Task.FromResult(false);

        File.Delete(fullFilePath);
        return Task.FromResult(true);
    }

    public Task<List<StorageFileInfo>> ListFilesAsync(
        string? path,
        CancellationToken cancellationToken = default)
    {
        var folderPath = ResolveFolderPath(path);
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
        var fullFilePath = ResolveFilePath(fileName, path);

        if (!File.Exists(fullFilePath))
            return null;

        var bytes = await File.ReadAllBytesAsync(fullFilePath, cancellationToken);
        return new MemoryStream(bytes);
    }

    public string GetFileUrl(string fileName, string? path = null)
    {
        return string.IsNullOrEmpty(path)
            ? $"{_baseUrl}/{fileName}"
            : $"{_baseUrl}/{path}/{fileName}";
    }

    private string ResolveFilePath(string fileName, string? folder)
    {
        if (string.IsNullOrWhiteSpace(fileName)
            || fileName != Path.GetFileName(fileName)
            || fileName is "." or "..")
        {
            throw new ArgumentException("Invalid file name.", nameof(fileName));
        }

        return Path.Combine(ResolveFolderPath(folder), fileName);
    }

    private string ResolveFolderPath(string? folder)
    {
        if (string.IsNullOrEmpty(folder))
            return _uploadsRootPath;

        var fullPath = Path.GetFullPath(Path.Combine(_uploadsRootPath, folder.Replace('/', Path.DirectorySeparatorChar)));
        var rootWithSeparator = _uploadsRootPath.EndsWith(Path.DirectorySeparatorChar)
            ? _uploadsRootPath
            : _uploadsRootPath + Path.DirectorySeparatorChar;

        if (!fullPath.StartsWith(rootWithSeparator, StringComparison.Ordinal))
            throw new ArgumentException("Invalid storage path.", nameof(folder));

        return fullPath;
    }
}
