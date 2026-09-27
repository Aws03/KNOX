namespace JadaraITKnowledgeSystem.Application.Common.Security;

/// <summary>
/// The application's roles, highest privilege first. A user holds exactly one role
/// and their JWT carries only that role, so the ordering here is the single source
/// of truth for "which role wins" and for who may grant what.
/// </summary>
public static class Roles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string Writer = "Writer";
    public const string User = "User";

    // Comma-separated forms for [Authorize(Roles = ...)].
    public const string AdminOrAbove = SuperAdmin + "," + Admin;
    public const string WriterOrAbove = SuperAdmin + "," + Admin + "," + Writer;

    public static IReadOnlyList<string> All { get; } = [SuperAdmin, Admin, Writer, User];

    /// <summary>Returns the highest-privilege role in <paramref name="roles"/>, or <see cref="User"/>.</summary>
    public static string Highest(IEnumerable<string> roles)
    {
        var held = roles as ICollection<string> ?? roles.ToList();
        return All.FirstOrDefault(role => held.Contains(role, StringComparer.OrdinalIgnoreCase)) ?? User;
    }

    /// <summary>Returns the canonical spelling of <paramref name="role"/>, or null if it isn't a known role.</summary>
    public static string? Normalize(string? role) =>
        All.FirstOrDefault(r => string.Equals(r, role?.Trim(), StringComparison.OrdinalIgnoreCase));
}
