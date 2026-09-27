using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

/// <summary>
/// Object storage over the S3 API. One client talks to the storage endpoint; a second one, configured
/// with the browser-facing endpoint, only signs URLs (presigning is offline, it never connects).
/// </summary>
public sealed partial class S3StorageService : IStorageService, IDisposable
{
    /// <summary>Objects are written once under unique keys, so browsers and the CDN may cache them indefinitely.</summary>
    public const string ImmutableCacheControl = "max-age=31536000, immutable";

    private readonly StorageOptions _options;
    private readonly AmazonS3Client _client;
    private readonly AmazonS3Client _signingClient;
    private readonly string _publicBaseUrl;

    public S3StorageService(IOptions<StorageOptions> options)
    {
        _options = options.Value;
        _client = CreateClient(_options, _options.ServiceUrl);
        _signingClient = _options.BrowserServiceUrl == _options.ServiceUrl
            ? _client
            : CreateClient(_options, _options.BrowserServiceUrl);
        _publicBaseUrl = _options.PublicBaseUrl.TrimEnd('/') + "/";
    }

    public async Task PutAsync(StorageBucket bucket, string key, Stream content, string contentType, CancellationToken cancellationToken = default)
    {
        var request = new PutObjectRequest
        {
            BucketName = BucketName(bucket),
            Key = ValidKey(key),
            InputStream = content,
            ContentType = contentType,
            AutoCloseStream = false
        };
        request.Headers.CacheControl = ImmutableCacheControl;

        await _client.PutObjectAsync(request, cancellationToken);
    }

    public async Task<Stream?> OpenReadAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.GetObjectAsync(BucketName(bucket), ValidKey(key), cancellationToken);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<StoredObject?> GetAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var metadata = await _client.GetObjectMetadataAsync(BucketName(bucket), ValidKey(key), cancellationToken);
            return new StoredObject(key, metadata.ContentLength, metadata.Headers.ContentType, ToOffset(metadata.LastModified));
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task CopyAsync(StorageBucket bucket, string sourceKey, string destinationKey, string contentType, CancellationToken cancellationToken = default)
    {
        var bucketName = BucketName(bucket);
        var request = new CopyObjectRequest
        {
            SourceBucket = bucketName,
            SourceKey = ValidKey(sourceKey),
            DestinationBucket = bucketName,
            DestinationKey = ValidKey(destinationKey),
            ContentType = contentType,
            MetadataDirective = S3MetadataDirective.REPLACE
        };
        request.Headers.CacheControl = ImmutableCacheControl;

        await _client.CopyObjectAsync(request, cancellationToken);
    }

    public async Task DeleteAsync(StorageBucket bucket, string key, CancellationToken cancellationToken = default) =>
        await _client.DeleteObjectAsync(BucketName(bucket), ValidKey(key), cancellationToken);

    public async IAsyncEnumerable<StoredObject> ListAsync(
        StorageBucket bucket,
        string prefix,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = new ListObjectsV2Request { BucketName = BucketName(bucket), Prefix = prefix };
        ListObjectsV2Response response;
        do
        {
            response = await _client.ListObjectsV2Async(request, cancellationToken);
            foreach (var item in response.S3Objects ?? [])
                yield return new StoredObject(item.Key, item.Size ?? 0, null, ToOffset(item.LastModified));

            request.ContinuationToken = response.NextContinuationToken;
        }
        while (response.IsTruncated == true);
    }

    public string GetPublicUrl(string key) => _publicBaseUrl + ValidKey(key);

    public string? GetPublicKey(string url)
    {
        if (!url.StartsWith(_publicBaseUrl, StringComparison.Ordinal))
            return null;

        var key = url[_publicBaseUrl.Length..];
        return KeyPattern().IsMatch(key) && !key.Contains("..") ? key : null;
    }

    public Uri CreateUploadUrl(StorageBucket bucket, string key, string contentType, DateTimeOffset expiresAt) =>
        Presign(BucketName(bucket), ValidKey(key), HttpVerb.PUT, contentType, expiresAt);

    public Uri CreateDownloadUrl(string key, DateTimeOffset expiresAt) =>
        _options.UsesCdnForPrivateObjects
            ? BunnyTokenSigner.Sign(_options.CdnBaseUrl!, ValidKey(key), _options.CdnTokenKey!, expiresAt)
            : Presign(_options.PrivateBucket, ValidKey(key), HttpVerb.GET, contentType: null, expiresAt);

    /// <summary>Lists one key from each bucket: proves the endpoint, credentials and both buckets work.</summary>
    public async Task CheckAvailabilityAsync(CancellationToken cancellationToken)
    {
        foreach (var bucket in new[] { _options.PublicBucket, _options.PrivateBucket })
            await _client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = bucket, MaxKeys = 1 }, cancellationToken);
    }

    public void Dispose()
    {
        _client.Dispose();
        if (!ReferenceEquals(_signingClient, _client))
            _signingClient.Dispose();
    }

    private Uri Presign(string bucketName, string key, HttpVerb verb, string? contentType, DateTimeOffset expiresAt)
    {
        var endpoint = new Uri(_options.BrowserServiceUrl);
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Verb = verb,
            Expires = expiresAt.UtcDateTime,
            Protocol = endpoint.Scheme == Uri.UriSchemeHttp ? Protocol.HTTP : Protocol.HTTPS
        };
        if (contentType is not null)
            request.ContentType = contentType;

        return new Uri(_signingClient.GetPreSignedURL(request));
    }

    private string BucketName(StorageBucket bucket) =>
        bucket == StorageBucket.Public ? _options.PublicBucket : _options.PrivateBucket;

    /// <summary>Keys come from request data (URLs, upload keys), so only plain relative paths are accepted.</summary>
    private static string ValidKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || !KeyPattern().IsMatch(key) || key.Contains(".."))
            throw new ArgumentException("Invalid storage key.", nameof(key));
        return key;
    }

    private static DateTimeOffset ToOffset(DateTime? value) =>
        value is { } v ? new DateTimeOffset(DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Utc)) : DateTimeOffset.MinValue;

    private static AmazonS3Client CreateClient(StorageOptions options, string serviceUrl)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = serviceUrl,
            ForcePathStyle = options.ForcePathStyle,
            AuthenticationRegion = options.Region,
            // Many S3-compatible services reject the SDK's default flexible checksums.
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
        };

        return string.IsNullOrWhiteSpace(options.AccessKey)
            ? new AmazonS3Client(config)
            : new AmazonS3Client(new BasicAWSCredentials(options.AccessKey, options.SecretKey), config);
    }

    [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9/_.-]{0,499}$")]
    private static partial Regex KeyPattern();
}
