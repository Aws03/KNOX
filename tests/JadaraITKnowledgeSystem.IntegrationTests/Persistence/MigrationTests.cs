using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;
using JadaraITKnowledgeSystem.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JadaraITKnowledgeSystem.IntegrationTests.Persistence;

[Collection(InfrastructureCollection.Name)]
public class MigrationTests(InfrastructureFixture database)
{
    private const string MigrationBeforeHardening = "20260714155111_RenameIsVerifiedColumns";

    [Fact]
    public async Task AllMigrationsApply_AndTheModelHasNoUnmigratedChanges()
    {
        await using var context = database.CreateContext();

        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        Assert.False(context.Database.HasPendingModelChanges());
    }

    [Fact]
    public async Task HardeningMigration_RepairsExistingDataInsteadOfFailing()
    {
        var connectionString = database.ConnectionStringFor("knox_upgrade");
        await using var context = database.CreateContext(connectionString: connectionString);
        await context.Database.EnsureDeletedAsync();
        await context.GetService<IMigrator>().MigrateAsync(MigrationBeforeHardening);

        // Data an existing production database can contain: duplicate reactions/attempts from
        // concurrent requests, counters that drifted, and plain-text refresh tokens.
        await context.Database.ExecuteSqlRawAsync("""
            DECLARE @now datetimeoffset = SYSDATETIMEOFFSET();
            INSERT INTO Universities (Name, CreatedAt) VALUES ('u', @now);
            INSERT INTO Faculties (Name, UniversityId, CreatedAt) VALUES ('f', SCOPE_IDENTITY(), @now);
            INSERT INTO Majors (Name, FacultyId, CreatedAt) VALUES ('m', SCOPE_IDENTITY(), @now);
            DECLARE @major int = SCOPE_IDENTITY();
            INSERT INTO Users (Name, Email, MajorId, IsActive, IsVerified, CreatedAt) VALUES ('a', 'A@X.COM', @major, 1, 1, @now);
            DECLARE @userA int = SCOPE_IDENTITY();
            INSERT INTO Users (Name, Email, MajorId, IsActive, IsVerified, CreatedAt) VALUES ('b', 'B@X.COM', @major, 1, 1, @now);
            DECLARE @userB int = SCOPE_IDENTITY();
            INSERT INTO Courses (CourseName, CreatedAt) VALUES ('c', @now);
            DECLARE @course int = SCOPE_IDENTITY();
            INSERT INTO CourseMaterials (Title, ContentUrl, CourseId, Tags, CreatedAt) VALUES
                ('local', 'http://localhost:5173/uploads/permanent/material/abc.pdf', @course, '[]', @now),
                ('bunny', 'https://jadara-hub.b-cdn.net/permanent/Lesson/talk.MP4', @course, '[]', @now);
            INSERT INTO Quizzes (CourseId, WriterId, Title, Likes, Dislikes, CreatedAt, Source, Tags)
                VALUES (@course, @userA, 'q', 7, 7, SYSUTCDATETIME(), 0, '[]');
            DECLARE @quiz int = SCOPE_IDENTITY();
            INSERT INTO UserReactions (UserId, QuizId, ReactionType, CreatedAt) VALUES
                (@userA, @quiz, 0, @now), (@userA, @quiz, 1, @now), (@userB, @quiz, 1, @now);
            INSERT INTO QuizAttempts (QuizId, UserId, Score, AttemptDate, CreatedAt) VALUES
                (@quiz, @userA, 40, DATEADD(day, -1, SYSUTCDATETIME()), @now), (@quiz, @userA, 90, SYSUTCDATETIME(), @now);
            INSERT INTO RefreshTokens (UserId, Token, ExpiresAt, CreatedAt, IsRevoked, CreatedByIp) VALUES
                (1, 'raw-1', DATEADD(day, 7, SYSUTCDATETIME()), SYSUTCDATETIME(), 0, 'ip'),
                (1, 'raw-2', DATEADD(day, 7, SYSUTCDATETIME()), SYSUTCDATETIME(), 0, 'ip');
            """);

        await context.Database.MigrateAsync();

        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        var quiz = await context.Quizzes.AsNoTracking().SingleAsync();
        Assert.Equal(2, quiz.Likes);     // the newest reaction per user was kept: both "like"
        Assert.Equal(0, quiz.Dislikes);
        Assert.Equal(90, (await context.QuizAttempts.AsNoTracking().SingleAsync()).Score);
        Assert.Empty(await context.RefreshTokens.ToListAsync());

        // Material URLs of the old storage became keys under legacy/ (where the files are copied to).
        var materials = await context.CourseMaterials.AsNoTracking().OrderBy(m => m.Title).ToListAsync();
        Assert.Equal(("legacy/permanent/Lesson/talk.MP4", "video/mp4"), (materials[0].StorageKey, materials[0].ContentType));
        Assert.Equal(("legacy/permanent/material/abc.pdf", "application/pdf"), (materials[1].StorageKey, materials[1].ContentType));
    }

    [Fact]
    public async Task MigrateCommand_ProvisionsALoginThatCanUseButNotChangeTheSchema()
    {
        const string login = "knox_app_test";
        const string password = "App-Login-Passw0rd!";
        var connectionString = database.ConnectionStringFor("knox_login");
        await using (var fresh = database.CreateContext(connectionString: connectionString))
            await fresh.Database.EnsureDeletedAsync();

        await using var api = new KnoxApiFactory(database, connectionString,
        [
            new("Database:AppLogin", login),
            new("Database:AppPassword", password)
        ]);
        // Twice: the deployment step must be idempotent (the second run re-keys the existing login).
        for (var run = 0; run < 2; run++)
        {
            await using var scope = api.Services.CreateAsyncScope();
            await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().MigrateAsync();
        }

        var appConnection = new SqlConnectionStringBuilder(connectionString) { UserID = login, Password = password }.ConnectionString;
        await using var connection = new SqlConnection(appConnection);
        await connection.OpenAsync();

        Assert.True(await ScalarAsync<int>(connection, "SELECT COUNT(*) FROM Universities") > 0);
        Assert.True(await ScalarAsync<int>(connection, "UPDATE Universities SET Name = Name; SELECT @@ROWCOUNT") > 0);
        var ddl = await Assert.ThrowsAsync<SqlException>(() => ScalarAsync<int>(connection, "CREATE TABLE Sneaky (Id int); SELECT 1"));
        Assert.Contains("permission", ddl.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<T> ScalarAsync<T>(SqlConnection connection, string sql)
    {
        await using var command = new SqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync())!;
    }
}
