using JadaraITKnowledgeSystem.IntegrationTests.TestSupport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JadaraITKnowledgeSystem.IntegrationTests.Persistence;

[Collection(SqlServerCollection.Name)]
public class MigrationTests(SqlServerFixture database)
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
            INSERT INTO Quizzes (CourseId, WriterId, Title, Likes, Dislikes, CreatedAt, Source, Tags)
                VALUES (SCOPE_IDENTITY(), @userA, 'q', 7, 7, SYSUTCDATETIME(), 0, '[]');
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
    }
}
