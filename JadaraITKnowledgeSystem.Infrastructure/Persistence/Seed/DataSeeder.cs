using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Seed;

/// <summary>
/// Seeds the minimum data needed to bootstrap a fresh database: one university,
/// one faculty, two majors, and a SuperAdmin account to log in with. Each step
/// checks for existing data first, so it's safe to run on every startup.
/// </summary>
public class DataSeeder(
    IApplicationDbContext context,
    IIdentityUserService identityService,
    UserManager<ApplicationUser> userManager,
    ILogger<DataSeeder> logger)
{
    private const string SuperAdminEmail = "admin@knox.com";
    private const string SuperAdminPassword = "Admin@123456";
    private const string SuperAdminFullName = "System Administrator";
    private const string SuperAdminRole = "SuperAdmin";

    private const string UniversityName = "Jadara University";
    private const string FacultyName = "Faculty of Information Technology";
    private static readonly string[] MajorNames = ["Computer Science", "Information Technology"];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var university = await GetOrCreateUniversityAsync(cancellationToken);
        var faculty = await GetOrCreateFacultyAsync(university.Id, cancellationToken);
        var majors = await GetOrCreateMajorsAsync(faculty.Id, cancellationToken);

        // Any seeded major works; the SuperAdmin's academic affiliation is nominal.
        await GetOrCreateSuperAdminAsync(majors[0].Id, cancellationToken);
    }

    private async Task<University> GetOrCreateUniversityAsync(CancellationToken ct)
    {
        // University.Create lower-cases the name before persisting, so compare
        // against that same normalized form rather than relying on DB collation.
        var storedName = UniversityName.Trim().ToLower();
        var existing = await context.Universities
            .FirstOrDefaultAsync(u => u.Name == storedName, ct);
        if (existing is not null)
            return existing;

        var result = University.Create(UniversityName);
        ThrowIfFailed(result, "seed university");

        await context.Universities.AddAsync(result.Value, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("[DataSeeder] Created university '{Name}'", UniversityName);
        return result.Value;
    }

    private async Task<Faculty> GetOrCreateFacultyAsync(int universityId, CancellationToken ct)
    {
        var existing = await context.Faculties
            .FirstOrDefaultAsync(f => f.UniversityId == universityId && f.Name == FacultyName, ct);
        if (existing is not null)
            return existing;

        var result = Faculty.Create(FacultyName, universityId);
        ThrowIfFailed(result, "seed faculty");

        await context.Faculties.AddAsync(result.Value, ct);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("[DataSeeder] Created faculty '{Name}'", FacultyName);
        return result.Value;
    }

    private async Task<List<Major>> GetOrCreateMajorsAsync(int facultyId, CancellationToken ct)
    {
        var majors = new List<Major>();

        foreach (var name in MajorNames)
        {
            var existing = await context.Majors
                .FirstOrDefaultAsync(m => m.FacultyId == facultyId && m.Name == name, ct);
            if (existing is not null)
            {
                majors.Add(existing);
                continue;
            }

            var result = Major.Create(name, facultyId);
            ThrowIfFailed(result, $"seed major '{name}'");

            await context.Majors.AddAsync(result.Value, ct);
            await context.SaveChangesAsync(ct);
            logger.LogInformation("[DataSeeder] Created major '{Name}'", name);
            majors.Add(result.Value);
        }

        return majors;
    }

    private async Task GetOrCreateSuperAdminAsync(int majorId, CancellationToken ct)
    {
        var existingIdentityUser = await userManager.FindByEmailAsync(SuperAdminEmail);
        if (existingIdentityUser is not null)
        {
            logger.LogInformation("[DataSeeder] SuperAdmin '{Email}' already exists, skipping", SuperAdminEmail);
            return;
        }

        var domainUserResult = User.Create(new FullName(SuperAdminFullName), new Email(SuperAdminEmail), majorId);
        ThrowIfFailed(domainUserResult, "seed SuperAdmin domain user");

        var domainUser = domainUserResult.Value;
        // A seeded admin account is trusted by definition - mark it verified so it
        // isn't blocked by the email-verification gate real registrations go through.
        domainUser.VerifyAccount();

        await context.Users.AddAsync(domainUser, ct);
        await context.SaveChangesAsync(ct);

        var identityCreate = await identityService.CreateAsync(
            SuperAdminEmail, SuperAdminFullName, domainUser.Id, SuperAdminPassword);

        // Note: CreateAsync's Result.IsSuccess is always true for this call (both
        // branches return through the value-tuple conversion) - the real signal is
        // an empty identityUserId / non-empty errors inside the tuple itself.
        var (identityUserId, identityErrors) = identityCreate.Value;
        if (identityUserId == 0 || identityErrors.Any())
        {
            context.Users.Remove(domainUser);
            await context.SaveChangesAsync(ct);
            throw new InvalidOperationException(
                $"[DataSeeder] Failed to create seed SuperAdmin identity user: " +
                string.Join("; ", identityErrors.Select(e => e.Description)));
        }

        var roleResult = await identityService.AddToRoleAsync(identityUserId, SuperAdminRole);

        if (!roleResult.IsSuccess)
        {
            context.Users.Remove(domainUser);
            await context.SaveChangesAsync(ct);
            throw new InvalidOperationException(
                $"[DataSeeder] Failed to assign '{SuperAdminRole}' role to seed SuperAdmin: " +
                string.Join("; ", roleResult.Errors.Select(e => e.Description)));
        }

        logger.LogInformation(
            "[DataSeeder] Created SuperAdmin account '{Email}' with role '{Role}'",
            SuperAdminEmail, SuperAdminRole);
    }

    private static void ThrowIfFailed<T>(Result<T> result, string what)
    {
        if (!result.IsSuccess)
        {
            throw new InvalidOperationException(
                $"[DataSeeder] Failed to create {what}: " +
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
