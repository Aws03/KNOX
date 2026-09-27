using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

/// <summary>A presigned direct upload: the client PUTs the file to <see cref="Url"/> with these headers.</summary>
public sealed record DirectUpload(string Key, Uri Url, string Method, IReadOnlyDictionary<string, string> Headers, DateTimeOffset ExpiresAt);

/// <summary>A course-material file that has been moved into its permanent location.</summary>
public sealed record MaterialFile(string Key, string ContentType, long Size);

/// <summary>
/// File policy on top of object storage.
/// Images (profile pictures, quiz images) are small, uploaded through the API and stored publicly under
/// permanent URLs. Course materials (documents and videos) are private: the browser uploads them straight
/// to storage and reads them through short-lived signed URLs.
/// </summary>
public interface IFileManager
{
    Task<string> UploadAsync(Stream fileStream, string extension, string folder, CancellationToken cancellationToken = default);

    /// <summary>Stores a new image and best-effort deletes the old one.</summary>
    Task<string> UpdateAsync(string? oldFileUrl, Stream fileStream, string extension, string folder, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default);

    /// <summary>Copies a temporary image (uploaded with folder "temp/...") to a permanent folder and returns its URL.</summary>
    Task<string> MoveFromTempToPermanentAsync(string tempFileUrl, string permanentFolder, CancellationToken cancellationToken = default);

    Task<int> DeleteOldTempFilesAsync(TimeSpan olderThan, CancellationToken cancellationToken = default);

    /// <summary>Validates a material file and returns a presigned upload into temporary storage.</summary>
    Result<DirectUpload> CreateMaterialUpload(string fileName, long size);

    /// <summary>Verifies an uploaded material and copies it to its permanent key under the course.</summary>
    Task<Result<MaterialFile>> ClaimMaterialUploadAsync(string uploadKey, int courseId, CancellationToken cancellationToken = default);

    /// <summary>A short-lived URL for reading a material.</summary>
    string GetMaterialUrl(string key);

    Task<Stream?> OpenMaterialAsync(string key, CancellationToken cancellationToken = default);

    Task DeleteMaterialAsync(string key, CancellationToken cancellationToken = default);
}
