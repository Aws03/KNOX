namespace JadaraITKnowledgeSystem.Infrastructure.Options;

/// <summary>Bootstrap SuperAdmin account; it is only created when a password is configured.</summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public string AdminEmail { get; set; } = "admin@knox.com";

    /// <summary>Set once for the first deployment (it must satisfy the password policy); change it after signing in.</summary>
    public string? AdminPassword { get; set; }
}
