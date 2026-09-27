using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Infrastructure.Options;

public sealed class JwtOptions
{
    public const string SectionName = "JwtSettings";

    /// <summary>HMAC-SHA256 signing key; at least 32 characters (256 bits).</summary>
    [Required, MinLength(32)]
    public string Secret { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int ExpirationMinutes { get; set; } = 30;

    [Range(1, 90)]
    public int RefreshTokenDays { get; set; } = 7;
}
