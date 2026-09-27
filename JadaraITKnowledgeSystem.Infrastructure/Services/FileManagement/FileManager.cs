using System.Text.RegularExpressions;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;

public sealed partial class FileManager(
    IStorageService storage,
    IOptions<StorageOptions> options,
    TimeProvider timeProvider,
    ILogger<FileManager> logger) : IFileManager
{
    private const string TempPrefix = "temp/";
    private const string MaterialUploadPrefix = "temp/materials/";

    private static readonly Dictionary<string, string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".gif"] = "image/gif",
        [".webp"] = "image/webp"
    };

    private static readonly Dictionary<string, string> MaterialTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pdf"] = "application/pdf",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".mp4"] = "video/mp4",
        [".webm"] = "video/webm"
    };

    private readonly StorageOptions _options = options.Value;

    public static IReadOnlyCollection<string> MaterialExtensions => MaterialTypes.Keys;

    // ---------- Public images ----------

    public async Task<string> UploadAsync(Stream fileStream, string extension, string folder, CancellationToken cancellationToken = default)
    {
        var contentType = ImageContentType(extension);
        if (!fileStream.CanRead || (fileStream.CanSeek && fileStream.Length == 0))
            throw new ArgumentException("The file is empty.", nameof(fileStream));

        var key = $"{folder.Trim('/')}/{NewName(extension)}";
        await storage.PutAsync(StorageBucket.Public, key, fileStream, contentType, cancellationToken);

        logger.LogInformation("Stored public file {Key}", key);
        return storage.GetPublicUrl(key);
    }

    public async Task<string> UpdateAsync(string? oldFileUrl, Stream fileStream, string extension, string folder, CancellationToken cancellationToken = default)
    {
        var newUrl = await UploadAsync(fileStream, extension, folder, cancellationToken);

        if (!string.IsNullOrWhiteSpace(oldFileUrl))
            await TryDeletePublicAsync(oldFileUrl);

        return newUrl;
    }

    public async Task<bool> DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default)
    {
        var key = string.IsNullOrWhiteSpace(fileUrl) ? null : storage.GetPublicKey(fileUrl);
        if (key is null)
        {
            logger.LogWarning("Not deleting {FileUrl}: it is not a file in this storage", fileUrl);
            return false;
        }

        await storage.DeleteAsync(StorageBucket.Public, key, cancellationToken);
        return true;
    }

    public async Task<string> MoveFromTempToPermanentAsync(string tempFileUrl, string permanentFolder, CancellationToken cancellationToken = default)
    {
        var tempKey = storage.GetPublicKey(tempFileUrl);
        if (tempKey is null || !tempKey.StartsWith(TempPrefix, StringComparison.Ordinal))
            throw new ArgumentException("The image URL is not a temporary upload.", nameof(tempFileUrl));

        var extension = Path.GetExtension(tempKey);
        var contentType = ImageContentType(extension);

        if (await storage.GetAsync(StorageBucket.Public, tempKey, cancellationToken) is null)
            throw new FileNotFoundException("The uploaded image no longer exists; upload it again.");

        var permanentKey = $"{permanentFolder.Trim('/')}/{NewName(extension)}";
        await storage.CopyAsync(StorageBucket.Public, tempKey, permanentKey, contentType, cancellationToken);
        await TryDeleteAsync(StorageBucket.Public, tempKey);

        return storage.GetPublicUrl(permanentKey);
    }

    /// <summary>Removes abandoned temporary uploads (quiz images never saved, materials never created).</summary>
    public async Task<int> DeleteOldTempFilesAsync(TimeSpan olderThan, CancellationToken cancellationToken = default)
    {
        var cutoff = timeProvider.GetUtcNow() - olderThan;
        var deleted = 0;

        foreach (var bucket in new[] { StorageBucket.Public, StorageBucket.Private })
        {
            await foreach (var item in storage.ListAsync(bucket, TempPrefix, cancellationToken))
            {
                if (item.LastModified >= cutoff)
                    continue;

                if (await TryDeleteAsync(bucket, item.Key))
                    deleted++;
            }
        }

        return deleted;
    }

    // ---------- Private course materials ----------

    public Result<DirectUpload> CreateMaterialUpload(string fileName, long size)
    {
        var extension = Path.GetExtension(fileName ?? string.Empty);
        if (!MaterialTypes.TryGetValue(extension, out var contentType))
            return Error.Validation("File.TypeNotAllowed",
                $"Allowed file types: {string.Join(", ", MaterialTypes.Keys)}.");

        if (size <= 0)
            return Error.Validation("File.Empty", "The file is empty.");

        if (size > _options.MaxMaterialBytes)
            return Error.Validation("File.TooLarge", $"Files may be at most {_options.MaxMaterialBytes / (1024 * 1024)} MB.");

        var key = MaterialUploadPrefix + NewName(extension);
        var expiresAt = timeProvider.GetUtcNow().AddMinutes(_options.UploadUrlMinutes);
        var url = storage.CreateUploadUrl(StorageBucket.Private, key, contentType, expiresAt);

        return new DirectUpload(key, url, "PUT", new Dictionary<string, string> { ["Content-Type"] = contentType }, expiresAt);
    }

    public async Task<Result<MaterialFile>> ClaimMaterialUploadAsync(string uploadKey, int courseId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(uploadKey) || !MaterialUploadKeyPattern().IsMatch(uploadKey))
            return Error.Validation("Upload.Invalid", "The upload key is not valid.");

        var extension = Path.GetExtension(uploadKey);
        if (!MaterialTypes.TryGetValue(extension, out var contentType))
            return Error.Validation("File.TypeNotAllowed", "The uploaded file type is not allowed.");

        var uploaded = await storage.GetAsync(StorageBucket.Private, uploadKey, cancellationToken);
        if (uploaded is null)
            return Error.Validation("Upload.NotFound", "The file was not uploaded or the upload expired; upload it again.");

        // A presigned PUT cannot limit the body size, so the limit is enforced here.
        if (uploaded.Size <= 0 || uploaded.Size > _options.MaxMaterialBytes)
        {
            await TryDeleteAsync(StorageBucket.Private, uploadKey);
            return Error.Validation("File.TooLarge", $"Files may be at most {_options.MaxMaterialBytes / (1024 * 1024)} MB.");
        }

        var key = $"materials/{courseId}/{NewName(extension)}";
        await storage.CopyAsync(StorageBucket.Private, uploadKey, key, contentType, cancellationToken);
        await TryDeleteAsync(StorageBucket.Private, uploadKey);

        logger.LogInformation("Stored material {Key} ({Size} bytes)", key, uploaded.Size);
        return new MaterialFile(key, contentType, uploaded.Size);
    }

    public string GetMaterialUrl(string key) =>
        storage.CreateDownloadUrl(key, timeProvider.GetUtcNow().AddMinutes(_options.DownloadUrlMinutes)).ToString();

    public Task<Stream?> OpenMaterialAsync(string key, CancellationToken cancellationToken = default) =>
        storage.OpenReadAsync(StorageBucket.Private, key, cancellationToken);

    public Task DeleteMaterialAsync(string key, CancellationToken cancellationToken = default) =>
        storage.DeleteAsync(StorageBucket.Private, key, cancellationToken);

    // ---------- Helpers ----------

    private static string ImageContentType(string extension) =>
        ImageTypes.TryGetValue(extension, out var contentType)
            ? contentType
            : throw new ArgumentException($"File type '{extension}' is not an allowed image type.", nameof(extension));

    private static string NewName(string extension) => $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";

    private async Task TryDeletePublicAsync(string url)
    {
        if (storage.GetPublicKey(url) is { } key)
            await TryDeleteAsync(StorageBucket.Public, key);
    }

    private async Task<bool> TryDeleteAsync(StorageBucket bucket, string key)
    {
        try
        {
            await storage.DeleteAsync(bucket, key, CancellationToken.None);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Best-effort delete of {Bucket}/{Key} failed", bucket, key);
            return false;
        }
    }

    [GeneratedRegex("^temp/materials/[0-9a-f]{32}\\.[a-z0-9]{2,5}$")]
    private static partial Regex MaterialUploadKeyPattern();
}
