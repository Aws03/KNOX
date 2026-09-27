using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

/// <summary>
/// Boots the real API (Program.cs: middleware, auth, controllers, migrations, seeding) against a
/// dedicated SQL Server database and the test object store.
/// </summary>
public sealed class KnoxApiFactory(
    InfrastructureFixture infrastructure,
    string connectionString,
    IEnumerable<KeyValuePair<string, string?>>? overrides = null) : WebApplicationFactory<Program>
{
    private readonly string _contentRoot = Directory.CreateTempSubdirectory("knox-api-tests-").FullName;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseContentRoot(_contentRoot);

        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Seed:AdminPassword"] = ApiClient.AdminPassword,
            // Small enough that a test can exceed it after a legitimate-looking upload request.
            ["Storage:MaxMaterialBytes"] = MaxMaterialBytes.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["ConnectionStrings:DefaultConnection"] = connectionString,
            ["Database:MigrateOnStartup"] = "true",
            ["JwtSettings:Secret"] = "integration-test-signing-key-0123456789abcdef",
            ["JwtSettings:Issuer"] = "knox-tests",
            ["JwtSettings:Audience"] = "knox-tests",
            ["OpenApi:Enabled"] = "true",
        }.Concat(infrastructure.StorageSettings()).Concat(overrides ?? [])));

        builder.ConfigureTestServices(services => services.AddTransient<IStartupFilter, DistinctClientIpStartupFilter>());
    }

    public const long MaxMaterialBytes = 1024 * 1024;

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            try { Directory.Delete(_contentRoot, recursive: true); } catch (IOException) { }
        }
    }

    /// <summary>
    /// TestServer requests carry no remote IP, so every request would share one rate-limit
    /// bucket. Give each request its own address so the per-IP auth limits don't couple tests.
    /// </summary>
    private sealed class DistinctClientIpStartupFilter : IStartupFilter
    {
        private static int _counter;

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app =>
        {
            app.Use((context, nextMiddleware) =>
            {
                var n = Interlocked.Increment(ref _counter);
                context.Connection.RemoteIpAddress = new IPAddress([10, (byte)(n >> 16), (byte)(n >> 8), (byte)n]);
                return nextMiddleware(context);
            });
            next(app);
        };
    }
}
