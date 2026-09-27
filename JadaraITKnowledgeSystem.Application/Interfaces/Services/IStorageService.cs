namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

/// <summary>Where an object lives: anyone may read public objects; private objects need a signed URL.</summary>
public enum StorageBucket
{
    Public,
    Private
}

public sealed record StoredObject(string Key, long Size, string? ContentType, DateTimeOffset LastModified);

/// <summary>
/// Object storage (S3-compatible). Keys are relative paths such as "materials/12/abc.mp4";
/// they never contain the bucket or host, so the same key works behind any CDN.
/// </summary>
public interface IStorageService
{
    Task PutAsync(StorageBucket bucket, string key, Stream content, string contentType, CancellationToken cancellationToken = default);

    /// <summary>Opens the object for reading, or returns null when it does not exist.</summary>
    Task<Stream?> OpenReadAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default);

    /// <summary>The object's metadata, or null when it does not exist.</summary>
    Task<StoredObject?> GetAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default);

    /// <summary>Server-side copy within a bucket; the copy gets the given content type and long-lived cache headers.</summary>
    Task CopyAsync(StorageBucket bucket, string sourceKey, string destinationKey, string contentType, CancellationToken cancellationToken = default);

    Task DeleteAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default);

    IAsyncEnumerable<StoredObject> ListAsync(StorageBucket bucket, string prefix, CancellationToken cancellationToken = default);

    /// <summary>Permanent URL of a public object.</summary>
    string GetPublicUrl(string key);

    /// <summary>The key of a URL produced by <see cref="GetPublicUrl"/>, or null for any other URL.</summary>
    string? GetPublicKey(string url);

    /// <summary>A URL the browser can PUT the object to directly, with exactly this Content-Type.</summary>
    Uri CreateUploadUrl(StorageBucket bucket, string key, string contentType, DateTimeOffset expiresAt);

    /// <summary>A short-lived URL for reading a private object (CDN token URL when configured, otherwise a presigned URL).</summary>
    Uri CreateDownloadUrl(string key, DateTimeOffset expiresAt);
}
