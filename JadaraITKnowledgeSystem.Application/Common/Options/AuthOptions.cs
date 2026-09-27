namespace JadaraITKnowledgeSystem.Application.Common.Options;

/// <summary>Bound from the "AuthSettings" configuration section.</summary>
public sealed class AuthOptions
{
    public const string SectionName = "AuthSettings";

    /// <summary>
    /// When true, new accounts must confirm their email via OTP before they can log in,
    /// and registration returns no tokens.
    /// </summary>
    public bool RequireEmailVerification { get; set; }
}
