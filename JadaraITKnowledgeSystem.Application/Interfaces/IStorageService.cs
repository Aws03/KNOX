namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
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

    string GetFileUrl(string fileName, string? path = null);
}

}
