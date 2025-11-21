using JadaraITKnowledgeSystem.Application.Common.Models;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IStorageService
{
    Task<string> UploadAsync(
        Stream fileStream,
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default);

    Task<List<StorageFileInfo>> ListFilesAsync(
        string? path,
        CancellationToken cancellationToken = default);

    Task<Stream?> DownloadAsync(
        string fileName,
        string? path = null,
        CancellationToken cancellationToken = default);

    string GetFileUrl(string fileName, string? path = null);


}
