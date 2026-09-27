using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Infrastructure.Interceptors;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

/// <summary>
/// Real infrastructure for the whole test run (Docker required): one SQL Server 2022 container and one
/// S3-compatible object store (SeaweedFS). Handler tests share one migrated database and isolate themselves
/// with unique data; the API host and the migration tests get databases of their own.
/// </summary>
public sealed class InfrastructureFixture : IAsyncLifetime
{
    public const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU27-ubuntu-22.04";
    public const string ObjectStoreImage = "chrislusf/seaweedfs:4.47";
    public const string PublicBucket = "knox-public";
    public const string PrivateBucket = "knox-private";
    public const string AccessKey = "knox-test";
    public const string SecretKey = "knox-test-secret";
    private const int S3Port = 8333;

    // Anonymous callers may only read the public bucket; the API's identity may do everything.
    private static readonly string S3Identities = $$"""
        {"identities":[
          {"name":"knox","credentials":[{"accessKey":"{{AccessKey}}","secretKey":"{{SecretKey}}"}],"actions":["Admin","Read","Write","List","Tagging"]},
          {"name":"anonymous","actions":["Read:{{PublicBucket}}"]}
        ]}
        """;

    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder(SqlServerImage).Build();

    private readonly IContainer _objectStore = new ContainerBuilder(ObjectStoreImage)
        .WithResourceMapping(Encoding.UTF8.GetBytes(S3Identities), "/etc/seaweedfs/s3.json")
        .WithCommand("server", "-dir=/data", "-s3", $"-s3.port={S3Port}", "-s3.config=/etc/seaweedfs/s3.json")
        .WithPortBinding(S3Port, assignRandomHostPort: true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(S3Port).ForPath("/healthz")))
        .Build();

    private KnoxApiFactory? _apiFactory;

    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>S3 endpoint reachable from the test process (and therefore from the in-process API).</summary>
    public string S3Endpoint { get; private set; } = string.Empty;

    /// <summary>The real API (Program.cs) running against its own database and the object store.</summary>
    public KnoxApiFactory Api => _apiFactory ??= new KnoxApiFactory(this, ConnectionStringFor("knox_api"));

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_sqlServer.StartAsync(), _objectStore.StartAsync());

        S3Endpoint = $"http://{_objectStore.Hostname}:{_objectStore.GetMappedPublicPort(S3Port)}";
        await CreateBucketsAsync();

        ConnectionString = ConnectionStringFor("knox_handlers");
        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_apiFactory is not null)
            await _apiFactory.DisposeAsync();
        await Task.WhenAll(_sqlServer.DisposeAsync().AsTask(), _objectStore.DisposeAsync().AsTask());
    }

    public string ConnectionStringFor(string database) =>
        new SqlConnectionStringBuilder(_sqlServer.GetConnectionString()) { InitialCatalog = database }.ConnectionString;

    /// <summary>Storage settings for the API, as an operator would configure them.</summary>
    public IEnumerable<KeyValuePair<string, string?>> StorageSettings() =>
    [
        new("Storage:ServiceUrl", S3Endpoint),
        new("Storage:AccessKey", AccessKey),
        new("Storage:SecretKey", SecretKey),
        new("Storage:PublicBucket", PublicBucket),
        new("Storage:PrivateBucket", PrivateBucket),
        new("Storage:PublicBaseUrl", $"{S3Endpoint}/{PublicBucket}")
    ];

    /// <summary>An S3 client with the API's credentials, for asserting what is (not) in the buckets.</summary>
    public AmazonS3Client CreateS3Client() =>
        new(AccessKey, SecretKey, new AmazonS3Config { ServiceURL = S3Endpoint, ForcePathStyle = true, AuthenticationRegion = "us-east-1" });

    /// <summary>A context configured like production (retrying execution strategy, audit interceptor).</summary>
    public AppDbContext CreateContext(ICurrentUserService? currentUser = null, string? connectionString = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString ?? ConnectionString, sql => sql.EnableRetryOnFailure())
            .AddInterceptors(new AuditableEntityInterceptor(currentUser ?? TestCurrentUser.Anonymous(), TimeProvider.System))
            .Options;

        return new AppDbContext(options);
    }

    private async Task CreateBucketsAsync()
    {
        using var s3 = CreateS3Client();
        foreach (var bucket in new[] { PublicBucket, PrivateBucket })
        {
            // The S3 gateway can report healthy a moment before the filer accepts writes.
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    await s3.PutBucketAsync(new PutBucketRequest { BucketName = bucket });
                    break;
                }
                catch (AmazonS3Exception) when (attempt < 20)
                {
                    await Task.Delay(500);
                }
            }
        }
    }
}

[CollectionDefinition(Name)]
public sealed class InfrastructureCollection : ICollectionFixture<InfrastructureFixture>
{
    public const string Name = "Infrastructure";
}
