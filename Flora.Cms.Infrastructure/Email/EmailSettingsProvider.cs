using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Options;
using Flora.Cms.Domain.Enums;
using EmailSettingsEntity = Flora.Cms.Domain.Entities.EmailSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Flora.Cms.Infrastructure.Email;

/// <summary>
/// Resolves effective delivery settings by layering the admin-saved row (source of truth,
/// secrets decrypted) over the server's appsettings <see cref="ContactOptions"/> fallback.
/// This keeps existing config-/user-secret-based setups working until an admin saves, and
/// lets an admin pick the provider in the UI while leaving secrets in user-secrets.
/// </summary>
public sealed class EmailSettingsProvider(
    IAppDbContext db,
    IEmailSecretProtector protector,
    IOptions<ContactOptions> contactOptions) : IEmailSettingsProvider
{
    public async Task<ResolvedEmailSettings> GetResolvedAsync(CancellationToken ct = default)
    {
        var row = await db.EmailSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == EmailSettingsEntity.SingletonId, ct);

        var opts = contactOptions.Value;

        var provider = row?.Provider ?? InferProvider(opts);

        return new ResolvedEmailSettings(
            provider,
            Coalesce(row?.FromAddress, opts.FromAddress) ?? "Flora Group <noreply@floragroup.net>",
            Coalesce(row?.SmtpHost, opts.SmtpHost),
            row?.SmtpPort ?? opts.SmtpPort,
            row?.SmtpUseSsl ?? opts.SmtpUseSsl,
            Coalesce(row?.SmtpUser, opts.SmtpUser),
            Coalesce(protector.Unprotect(row?.SmtpPasswordEncrypted), opts.SmtpPassword),
            Coalesce(protector.Unprotect(row?.ResendApiKeyEncrypted), opts.ResendApiKey));
    }

    /// <summary>Before an admin saves, mirror the old startup behaviour: Resend if a key
    /// exists, else SMTP if a host exists, else log-only (never sends by accident).</summary>
    private static EmailProvider InferProvider(ContactOptions o)
        => !string.IsNullOrWhiteSpace(o.ResendApiKey) ? EmailProvider.RESEND
            : !string.IsNullOrWhiteSpace(o.SmtpHost) ? EmailProvider.SMTP
            : EmailProvider.LOG;

    private static string? Coalesce(string? primary, string? fallback)
        => !string.IsNullOrWhiteSpace(primary) ? primary
            : string.IsNullOrWhiteSpace(fallback) ? null : fallback;
}
