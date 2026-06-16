using System.ComponentModel.DataAnnotations;

namespace Flora.Cms.Infrastructure.Security;

/// <summary>Strongly-typed JWT settings bound from the "Jwt" configuration section.</summary>
public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Signing key. MUST be supplied via a secret (user-secrets / env var /
    /// key vault) in any non-development environment — never committed.
    /// HMAC-SHA256 requires at least 256 bits (32 ASCII chars).
    /// </summary>
    [Required]
    [MinLength(32)]
    public string SigningKey { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int AccessTokenMinutes { get; set; } = 60;
}
