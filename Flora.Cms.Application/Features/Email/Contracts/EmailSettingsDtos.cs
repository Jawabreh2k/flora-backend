using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Email.Contracts;

/// <summary>
/// Email delivery settings as exposed to the admin portal. Secrets are NEVER returned —
/// only a boolean indicating whether each one is currently set.
/// </summary>
public record EmailSettingsDto(
    EmailProvider Provider,
    string FromAddress,
    string? SmtpHost,
    int SmtpPort,
    bool SmtpUseSsl,
    string? SmtpUser,
    bool SmtpPasswordSet,
    bool ResendApiKeySet,
    DateTimeOffset? UpdatedAtUtc,
    string? UpdatedBy);

/// <summary>
/// Admin update payload. Secret fields use leave-blank-to-keep semantics: a null/empty
/// value preserves the stored secret; a non-empty value replaces it.
/// </summary>
public record UpdateEmailSettingsInput(
    EmailProvider Provider,
    string FromAddress,
    string? SmtpHost,
    int SmtpPort,
    bool SmtpUseSsl,
    string? SmtpUser,
    string? SmtpPassword,
    string? ResendApiKey);

/// <summary>Outcome of a "send test email" attempt, surfaced inline in the UI.</summary>
public record TestEmailResultDto(bool Success, EmailProvider Provider, string? Error);
