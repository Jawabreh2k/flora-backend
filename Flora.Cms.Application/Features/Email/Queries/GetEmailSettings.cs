using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Email.Contracts;
using Flora.Cms.Domain.Entities;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Email.Queries;

/// <summary>Reads the admin email delivery settings (secrets masked to booleans).</summary>
public sealed record GetEmailSettingsQuery : IQuery<EmailSettingsDto>;

public sealed class GetEmailSettingsQueryHandler(IAppDbContext db)
    : IQueryHandler<GetEmailSettingsQuery, EmailSettingsDto>
{
    public async Task<EmailSettingsDto> Handle(GetEmailSettingsQuery query, CancellationToken ct)
    {
        var row = await db.EmailSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == EmailSettings.SingletonId, ct);

        if (row is null)
        {
            // Not configured yet — return safe defaults so the form renders sensibly.
            return new EmailSettingsDto(
                EmailProvider.LOG,
                "Flora Group <noreply@floragroup.net>",
                SmtpHost: null,
                SmtpPort: 587,
                SmtpUseSsl: true,
                SmtpUser: null,
                SmtpPasswordSet: false,
                ResendApiKeySet: false,
                UpdatedAtUtc: null,
                UpdatedBy: null);
        }

        return new EmailSettingsDto(
            row.Provider,
            row.FromAddress,
            row.SmtpHost,
            row.SmtpPort,
            row.SmtpUseSsl,
            row.SmtpUser,
            SmtpPasswordSet: !string.IsNullOrEmpty(row.SmtpPasswordEncrypted),
            ResendApiKeySet: !string.IsNullOrEmpty(row.ResendApiKeyEncrypted),
            row.UpdatedAtUtc,
            row.UpdatedBy);
    }
}
