using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Context;

namespace JadaraITKnowledgeSystem.IntegrationTests.TestSupport;

/// <summary>Seed helpers that use unique names, since handler tests share one database.</summary>
public static class TestData
{
    public static string Unique(string prefix) => $"{prefix}-{Guid.NewGuid():N}"[..Math.Min(prefix.Length + 13, 60)];

    public static string UniqueEmail() => $"{Guid.NewGuid():N}@test.local";

    public static async Task<(University University, Faculty Faculty, Major Major)> SeedHierarchyAsync(this AppDbContext context)
    {
        var university = University.Create(Unique("University")).Value;
        context.Universities.Add(university);
        await context.SaveChangesAsync();

        var faculty = Faculty.Create(Unique("Faculty"), university.Id).Value;
        context.Faculties.Add(faculty);
        await context.SaveChangesAsync();

        var major = Major.Create(Unique("Major"), faculty.Id).Value;
        context.Majors.Add(major);
        await context.SaveChangesAsync();

        return (university, faculty, major);
    }

    public static async Task<User> SeedUserAsync(this AppDbContext context, int majorId, string? email = null, string name = "Test User")
    {
        var user = User.Create(new FullName(name), new Email(email ?? UniqueEmail()), majorId).Value;
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }
}
