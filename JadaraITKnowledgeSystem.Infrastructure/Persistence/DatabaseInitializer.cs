using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence;

/// <summary>Startup database work: optional migrations, then idempotent seeding.</summary>
public sealed class DatabaseInitializer(
    AppDbContext context,
    RoleSeeder roleSeeder,
    DataSeeder dataSeeder,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (options.Value.MigrateOnStartup)
        {
            var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
            if (pending.Count > 0)
            {
                logger.LogInformation("Applying {Count} pending migration(s): {Migrations}", pending.Count, string.Join(", ", pending));
                await context.Database.MigrateAsync(cancellationToken);
            }
        }

        if (options.Value.SeedOnStartup)
        {
            // Roles first: the seeded SuperAdmin needs the "SuperAdmin" role to exist.
            await roleSeeder.SeedAsync();
            await dataSeeder.SeedAsync(cancellationToken);
        }
    }
}
