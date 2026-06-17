using System.Text.Json;
using Flora.Cms.Application.Abstractions.Contact;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.UiConfig;
using Flora.Cms.Application.Options;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Flora.Cms.Application.Features.Contact;

public sealed class ContactRecipientResolver(
    IAppDbContext db,
    IOptions<ContactOptions> options) : IContactRecipientResolver
{
    public async Task<string> ResolveAsync(string locale, CancellationToken ct = default)
    {
        var normalized = NormalizeLocale(locale);

        var entity = await db.UiConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct);

        if (entity is not null)
        {
            var fromCms = TryReadFromPayload(entity.PayloadJson, normalized);
            if (!string.IsNullOrWhiteSpace(fromCms))
                return fromCms.Trim();
        }

        var fromConfig = options.Value.RecipientEmail;
        return string.IsNullOrWhiteSpace(fromConfig)
            ? "info@floragroup.net"
            : fromConfig.Trim();
    }

    public static string? TryReadFromPayload(string payloadJson, string locale)
        => TryReadEmailFromPayload(payloadJson, locale, "contact", "formRecipientEmail");

    /// <summary>
    /// Reads <c>content.{locale}.{group}.{key}</c> from the CMS payload, trying the
    /// requested locale then en/ar. Returns null when absent or corrupted. Shared by the
    /// contact and careers recipient resolvers.
    /// </summary>
    public static string? TryReadEmailFromPayload(string payloadJson, string locale, string group, string key)
    {
        try
        {
            using var doc = JsonDocument.Parse(payloadJson);
            if (!doc.RootElement.TryGetProperty("content", out var content) ||
                content.ValueKind is not JsonValueKind.Object)
            {
                return null;
            }

            foreach (var loc in new[] { locale, "en", "ar" })
            {
                if (!content.TryGetProperty(loc, out var localeEl) ||
                    localeEl.ValueKind is not JsonValueKind.Object)
                {
                    continue;
                }

                if (!localeEl.TryGetProperty(group, out var groupEl) ||
                    groupEl.ValueKind is not JsonValueKind.Object)
                {
                    continue;
                }

                if (!groupEl.TryGetProperty(key, out var emailProp))
                {
                    continue;
                }

                var value = emailProp.GetString();
                if (!string.IsNullOrWhiteSpace(value) && !ContentMerger.IsCorrupted(value))
                {
                    return value;
                }
            }
        }
        catch (JsonException)
        {
            // Fall through to server default.
        }

        return null;
    }

    private static string NormalizeLocale(string? locale)
        => locale?.Trim().ToLowerInvariant() == "ar" ? "ar" : "en";
}
