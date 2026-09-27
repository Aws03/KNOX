using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Infrastructure.Options;

/// <summary>
/// S3-compatible object storage (AWS S3, Cloudflare R2, Backblaze B2, MinIO, ...) with an optional CDN.
/// Two buckets: public objects (images) are read anonymously via <see cref="PublicBaseUrl"/>; private objects
/// (course materials, including video) are only reachable through short-lived signed URLs.
/// </summary>
public sealed class StorageOptions : IValidatableObject
{
    public const string SectionName = "Storage";

    /// <summary>S3 endpoint the API talks to, e.g. https://&lt;account&gt;.r2.cloudflarestorage.com or http://minio:9000.</summary>
    [Required, Url]
    public string ServiceUrl { get; set; } = string.Empty;

    /// <summary>
    /// S3 endpoint browsers use for presigned uploads/downloads, when it differs from <see cref="ServiceUrl"/>
    /// (e.g. the API reaches MinIO on a private network but browsers use its public hostname).
    /// </summary>
    public string? PublicServiceUrl { get; set; }

    /// <summary>Signing region; "auto" for Cloudflare R2, "us-east-1" for MinIO.</summary>
    [Required]
    public string Region { get; set; } = "us-east-1";

    /// <summary>Access key; leave empty to use the AWS default credential chain (environment, instance role).</summary>
    public string? AccessKey { get; set; }

    public string? SecretKey { get; set; }

    /// <summary>Path-style addressing (endpoint/bucket/key); required by MinIO and most S3-compatible services.</summary>
    public bool ForcePathStyle { get; set; } = true;

    [Required]
    public string PublicBucket { get; set; } = string.Empty;

    [Required]
    public string PrivateBucket { get; set; } = string.Empty;

    /// <summary>Base URL for public objects: a CDN pull zone in front of the public bucket, or the bucket's own public URL.</summary>
    [Required, Url]
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>Optional Bunny CDN pull zone in front of the private bucket, with token authentication enabled.</summary>
    public string? CdnBaseUrl { get; set; }

    /// <summary>The pull zone's token authentication key (required together with <see cref="CdnBaseUrl"/>).</summary>
    public string? CdnTokenKey { get; set; }

    /// <summary>Largest course material (video) accepted. A single presigned PUT and copy is limited to 5 GiB.</summary>
    [Range(1, 5L * 1024 * 1024 * 1024)]
    public long MaxMaterialBytes { get; set; } = 2L * 1024 * 1024 * 1024;

    [Range(1, 60 * 24)]
    public int UploadUrlMinutes { get; set; } = 60;

    [Range(1, 60 * 24 * 7)]
    public int DownloadUrlMinutes { get; set; } = 240;

    public bool UsesCdnForPrivateObjects => !string.IsNullOrWhiteSpace(CdnBaseUrl);

    public string BrowserServiceUrl => string.IsNullOrWhiteSpace(PublicServiceUrl) ? ServiceUrl : PublicServiceUrl;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var (name, value) in new[] { (nameof(PublicServiceUrl), PublicServiceUrl), (nameof(CdnBaseUrl), CdnBaseUrl) })
        {
            if (!string.IsNullOrWhiteSpace(value) && !Uri.TryCreate(value, UriKind.Absolute, out _))
                yield return new ValidationResult($"Storage:{name} must be an absolute URL.", [name]);
        }

        if (string.IsNullOrWhiteSpace(CdnBaseUrl) != string.IsNullOrWhiteSpace(CdnTokenKey))
            yield return new ValidationResult(
                "Storage:CdnBaseUrl and Storage:CdnTokenKey must be set together.",
                [nameof(CdnBaseUrl), nameof(CdnTokenKey)]);

        if (string.IsNullOrWhiteSpace(AccessKey) != string.IsNullOrWhiteSpace(SecretKey))
            yield return new ValidationResult(
                "Storage:AccessKey and Storage:SecretKey must be set together.",
                [nameof(AccessKey), nameof(SecretKey)]);

        if (PublicBucket == PrivateBucket)
            yield return new ValidationResult(
                "Storage:PublicBucket and Storage:PrivateBucket must be different buckets.",
                [nameof(PublicBucket), nameof(PrivateBucket)]);
    }
}
