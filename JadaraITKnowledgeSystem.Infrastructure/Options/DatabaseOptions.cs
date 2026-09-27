using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>Apply pending EF Core migrations at startup (development). Production runs the "migrate" command instead.</summary>
    public bool MigrateOnStartup { get; set; }

    /// <summary>Create roles and the bootstrap university/SuperAdmin data at startup (idempotent).</summary>
    public bool SeedOnStartup { get; set; } = true;

    /// <summary>
    /// Least-privilege SQL login for the running API (db_datareader + db_datawriter), created or updated by the
    /// "migrate" command, which itself connects with an owner account. Leave empty to manage logins yourself.
    /// </summary>
    [MaxLength(128)]
    public string? AppLogin { get; set; }

    [MaxLength(128)]
    public string? AppPassword { get; set; }
}
