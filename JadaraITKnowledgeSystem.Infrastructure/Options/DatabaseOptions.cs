namespace JadaraITKnowledgeSystem.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>Apply pending EF Core migrations at startup (single-instance deployments).</summary>
    public bool MigrateOnStartup { get; set; }

    /// <summary>Create roles and the bootstrap university/SuperAdmin data at startup (idempotent).</summary>
    public bool SeedOnStartup { get; set; } = true;
}
