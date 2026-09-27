using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Infrastructure.Interceptors;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

/// <summary>
/// One SQL Server 2022 container for the whole test run (Docker required). Handler tests share
/// one migrated database and isolate themselves with unique data; the API host and the
/// migration-upgrade test get databases of their own.
/// </summary>
public sealed class SqlServerFixture : IAsyncLifetime
{
    public const string Image = "mcr.microsoft.com/mssql/server:2022-latest";

    private readonly MsSqlContainer _container = new MsSqlBuilder(Image).Build();
    private KnoxApiFactory? _apiFactory;

    public string ConnectionString { get; private set; } = string.Empty;

    /// <summary>The real API (Program.cs) running against its own database.</summary>
    public KnoxApiFactory Api => _apiFactory ??= new KnoxApiFactory(ConnectionStringFor("knox_api"));

    public async Task InitializeAsync()
    {
        await _container.StartAsync();
        ConnectionString = ConnectionStringFor("knox_handlers");

        await using var context = CreateContext();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_apiFactory is not null)
            await _apiFactory.DisposeAsync();
        await _container.DisposeAsync();
    }

    public string ConnectionStringFor(string database) =>
        new SqlConnectionStringBuilder(_container.GetConnectionString()) { InitialCatalog = database }.ConnectionString;

    /// <summary>A context configured like production (retrying execution strategy, audit interceptor).</summary>
    public AppDbContext CreateContext(ICurrentUserService? currentUser = null, string? connectionString = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString ?? ConnectionString, sql => sql.EnableRetryOnFailure())
            .AddInterceptors(new AuditableEntityInterceptor(currentUser ?? TestCurrentUser.Anonymous(), TimeProvider.System))
            .Options;

        return new AppDbContext(options);
    }
}

[CollectionDefinition(Name)]
public sealed class SqlServerCollection : ICollectionFixture<SqlServerFixture>
{
    public const string Name = "SqlServer";
}
