using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;

/// <summary>
/// Lets `dotnet ef` create the context without booting the API. Migrations scaffolding
/// doesn't need a live database; `database update` uses KNOX_MIGRATIONS_CONNECTION.
/// </summary>
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("KNOX_MIGRATIONS_CONNECTION")
            ?? "Server=localhost,1433;Database=JadaraITKnowledgeSystemDB;Integrated Security=false;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
