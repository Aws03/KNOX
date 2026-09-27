using JadaraITKnowledgeSystem.Domain.Quizzes.Enums;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence;

/// <summary>
/// Database lifecycle. <see cref="MigrateAsync"/> is the deployment step (the "migrate" command, run with an
/// owner account before the new version starts); <see cref="InitializeAsync"/> runs on every API start.
/// </summary>
public sealed class DatabaseInitializer(
    AppDbContext context,
    RoleSeeder roleSeeder,
    DataSeeder dataSeeder,
    IOptions<DatabaseOptions> options,
    ILogger<DatabaseInitializer> logger)
{
    private static readonly QuizGenerationStatus[] InProgress =
        [QuizGenerationStatus.Pending, QuizGenerationStatus.Extracting, QuizGenerationStatus.GeneratingQuizzes];

    /// <summary>Applies migrations, provisions the API's SQL login and seeds bootstrap data.</summary>
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        LogTarget();
        await ApplyMigrationsAsync(cancellationToken);
        await EnsureAppLoginAsync(cancellationToken);
        await SeedAsync(cancellationToken);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        LogTarget();
        if (options.Value.MigrateOnStartup)
            await ApplyMigrationsAsync(cancellationToken);

        if (options.Value.SeedOnStartup)
            await SeedAsync(cancellationToken);

        await FailInterruptedJobsAsync(cancellationToken);
    }

    /// <summary>
    /// Names the server and database in use (never the credentials): configuration layers such as
    /// dotnet user-secrets or environment variables can silently override appsettings.
    /// </summary>
    private void LogTarget()
    {
        var connection = context.Database.GetDbConnection();
        logger.LogInformation("Using database {Database} on {Server}", connection.Database, connection.DataSource);
    }

    private async Task ApplyMigrationsAsync(CancellationToken cancellationToken)
    {
        var pending = (await context.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (pending.Count == 0)
        {
            logger.LogInformation("Database schema is up to date");
            return;
        }

        logger.LogInformation("Applying {Count} pending migration(s): {Migrations}", pending.Count, string.Join(", ", pending));
        await context.Database.MigrateAsync(cancellationToken);
    }

    private async Task SeedAsync(CancellationToken cancellationToken)
    {
        // Roles first: the seeded SuperAdmin needs the "SuperAdmin" role to exist.
        await roleSeeder.SeedAsync();
        await dataSeeder.SeedAsync(cancellationToken);
    }

    /// <summary>
    /// Creates (or re-keys) a login that can only read and write data, so the running API never holds
    /// schema or server permissions. Idempotent; names and passwords are quoted, never concatenated raw.
    /// </summary>
    private async Task EnsureAppLoginAsync(CancellationToken cancellationToken)
    {
        var login = options.Value.AppLogin;
        var password = options.Value.AppPassword;
        if (string.IsNullOrWhiteSpace(login))
            return;
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException("Database:AppPassword is required when Database:AppLogin is set.");

        await context.Database.ExecuteSqlAsync($"""
            DECLARE @login sysname = {login}, @password nvarchar(128) = {password}, @sql nvarchar(max);
            IF SUSER_ID(@login) IS NULL
                SET @sql = N'CREATE LOGIN ' + QUOTENAME(@login) + N' WITH PASSWORD = ' + QUOTENAME(@password, '''') + N', CHECK_POLICY = ON;';
            ELSE
                SET @sql = N'ALTER LOGIN ' + QUOTENAME(@login) + N' WITH PASSWORD = ' + QUOTENAME(@password, '''') + N';';
            IF USER_ID(@login) IS NULL
                SET @sql += N'CREATE USER ' + QUOTENAME(@login) + N' FOR LOGIN ' + QUOTENAME(@login) + N';';
            ELSE -- re-link after a backup was restored on another server (the user would be orphaned)
                SET @sql += N'ALTER USER ' + QUOTENAME(@login) + N' WITH LOGIN = ' + QUOTENAME(@login) + N';';
            SET @sql += N'ALTER ROLE db_datareader ADD MEMBER ' + QUOTENAME(@login) + N';'
                      + N'ALTER ROLE db_datawriter ADD MEMBER ' + QUOTENAME(@login) + N';';
            EXEC sys.sp_executesql @sql;
            """, cancellationToken);

        logger.LogInformation("SQL login {Login} has read/write access to the database", login);
    }

    /// <summary>
    /// Quiz generation runs on an in-process queue, so jobs that were queued or running when the process
    /// stopped can never finish. Failing them tells the writer to start again instead of polling forever.
    /// </summary>
    private async Task FailInterruptedJobsAsync(CancellationToken cancellationToken)
    {
        var interrupted = await context.QuizGenerationJobs
            .Where(j => InProgress.Contains(j.Status))
            .ToListAsync(cancellationToken);
        if (interrupted.Count == 0)
            return;

        foreach (var job in interrupted)
            job.MarkFailed("Interrupted by a server restart. Start the generation again.");

        await context.SaveChangesAsync(cancellationToken);
        logger.LogWarning("Marked {Count} interrupted quiz generation job(s) as failed", interrupted.Count);
    }
}
