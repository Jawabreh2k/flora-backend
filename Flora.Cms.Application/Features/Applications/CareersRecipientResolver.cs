using Flora.Cms.Application.Abstractions.Careers;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Contact;
using Flora.Cms.Application.Options;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Flora.Cms.Application.Features.Applications;

/// <summary>
/// Resolves the new-application alert recipient from CMS config: the careers HR email
/// (<c>careersCta.email</c>), falling back to the contact inbox, then the server default.
/// </summary>
public sealed class CareersRecipientResolver(
    IAppDbContext db,
    IOptions<ContactOptions> options) : ICareersRecipientResolver
{
    public async Task<string> ResolveAsync(string locale, CancellationToken ct = default)
    {
        var normalized = NormalizeLocale(locale);

        var entity = await db.UiConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct);

        if (entity is not null)
        {
            var hr = ContactRecipientResolver.TryReadEmailFromPayload(entity.PayloadJson, normalized, "careersCta", "email");
            if (!string.IsNullOrWhiteSpace(hr))
                return hr.Trim();

            // Fall back to the general contact inbox if no dedicated careers email is set.
            var contact = ContactRecipientResolver.TryReadEmailFromPayload(entity.PayloadJson, normalized, "contact", "formRecipientEmail");
            if (!string.IsNullOrWhiteSpace(contact))
                return contact.Trim();
        }

        var fromConfig = options.Value.RecipientEmail;
        return string.IsNullOrWhiteSpace(fromConfig)
            ? "careers@floragroup.net"
            : fromConfig.Trim();
    }

    private static string NormalizeLocale(string? locale)
        => locale?.Trim().ToLowerInvariant() == "ar" ? "ar" : "en";
}
