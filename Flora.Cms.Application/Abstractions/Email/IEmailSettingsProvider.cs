using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Abstractions.Email;

/// <summary>
/// Fully-resolved, ready-to-use delivery settings: the admin-saved values merged over the
/// server's appsettings fallback, with secrets already decrypted. Consumed only by the
/// email sender at send time — never serialized to a client.
/// </summary>
public sealed record ResolvedEmailSettings(
    EmailProvider Provider,
    string FromAddress,
    string? SmtpHost,
    int SmtpPort,
    bool SmtpUseSsl,
    string? SmtpUser,
    string? SmtpPassword,
    string? ResendApiKey);

/// <summary>Resolves the effective email delivery settings for the current request.</summary>
public interface IEmailSettingsProvider
{
    Task<ResolvedEmailSettings> GetResolvedAsync(CancellationToken ct = default);
}
