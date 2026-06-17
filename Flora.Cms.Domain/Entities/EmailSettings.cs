using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Domain.Entities;

/// <summary>
/// Singleton row holding the admin-managed email delivery configuration. Kept in its
/// own table — deliberately NOT part of the public <see cref="UiConfiguration"/> payload —
/// because it carries secrets (SMTP password, Resend API key) that must never be served
/// to the public website. Secret columns store ciphertext; plaintext never leaves the API.
/// </summary>
public class EmailSettings
{
    /// <summary>The fixed primary-key value for the one-and-only settings row.</summary>
    public const int SingletonId = 1;

    public int Id { get; set; } = SingletonId;

    public EmailProvider Provider { get; set; } = EmailProvider.LOG;

    public string FromAddress { get; set; } = "Flora Group <noreply@floragroup.net>";

    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public bool SmtpUseSsl { get; set; } = true;
    public string? SmtpUser { get; set; }

    /// <summary>Data-Protection–encrypted SMTP password. Never returned by the API.</summary>
    public string? SmtpPasswordEncrypted { get; set; }

    /// <summary>Data-Protection–encrypted Resend API key. Never returned by the API.</summary>
    public string? ResendApiKeyEncrypted { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? UpdatedBy { get; set; }
}
