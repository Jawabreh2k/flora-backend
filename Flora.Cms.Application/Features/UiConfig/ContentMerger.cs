using System.Text.Json;

namespace Flora.Cms.Application.Features.UiConfig;

/// <summary>
/// Merges a stored content JSON element with the built-in bilingual defaults.
/// Kept free of any database dependency so it can be unit-tested in isolation, and
/// to make write-time healing explicit.
/// </summary>
public static class ContentMerger
{
    /// <summary>
    /// Returns a <see cref="JsonElement"/> where every locale/group/field from the
    /// bundled defaults is present. Rules (applied in priority order):
    /// <list type="number">
    ///   <item>If the existing element is empty/null, return the full defaults.</item>
    ///   <item>If a locale is missing, back-fill the entire locale from defaults.</item>
    ///   <item>If a group is missing, back-fill the entire group from defaults.</item>
    ///   <item>If a field value is Unicode-corrupted (only '?' chars), replace with the default.</item>
    ///   <item>User-added groups that are not in the defaults are carried over unchanged.</item>
    /// </list>
    /// </summary>
    public static JsonElement MergeDefaults(JsonElement existing)
    {
        using var defaultDoc = JsonDocument.Parse(DefaultContent.Json);
        var defaults = defaultDoc.RootElement;

        if (existing.ValueKind is not JsonValueKind.Object)
            return defaults.Clone();

        var result = new Dictionary<string, object?>();

        foreach (var localeProp in defaults.EnumerateObject())
        {
            var locale = localeProp.Name;
            var defaultGroups = localeProp.Value;

            if (!existing.TryGetProperty(locale, out var existingLocale) ||
                existingLocale.ValueKind is not JsonValueKind.Object)
            {
                result[locale] = JsonDocument.Parse(defaultGroups.GetRawText()).RootElement.Clone();
                continue;
            }

            var mergedLocale = new Dictionary<string, object?>();

            foreach (var defaultGrpProp in defaultGroups.EnumerateObject())
            {
                var grpName = defaultGrpProp.Name;
                var defaultGrp = defaultGrpProp.Value;

                if (!existingLocale.TryGetProperty(grpName, out var existingGrp) ||
                    existingGrp.ValueKind is not JsonValueKind.Object)
                {
                    mergedLocale[grpName] = JsonDocument.Parse(defaultGrp.GetRawText()).RootElement.Clone();
                    continue;
                }

                var mergedGrp = new Dictionary<string, string>();
                foreach (var fieldProp in defaultGrp.EnumerateObject())
                {
                    var key = fieldProp.Name;
                    var defaultVal = fieldProp.Value.GetString() ?? "";

                    if (existingGrp.TryGetProperty(key, out var existingField))
                    {
                        var existingVal = existingField.GetString() ?? "";
                        mergedGrp[key] = IsCorrupted(existingVal) ? defaultVal : existingVal;
                    }
                    else
                    {
                        mergedGrp[key] = defaultVal;
                    }
                }
                mergedLocale[grpName] = mergedGrp;
            }

            // Preserve any extra groups the user added beyond the defaults.
            foreach (var grp in existingLocale.EnumerateObject())
            {
                if (!mergedLocale.ContainsKey(grp.Name))
                    mergedLocale[grp.Name] = JsonDocument.Parse(grp.Value.GetRawText()).RootElement.Clone();
            }

            result[locale] = mergedLocale;
        }

        var json = JsonSerializer.Serialize(result);
        return JsonDocument.Parse(json).RootElement.Clone();
    }

    /// <summary>
    /// Returns <see langword="true"/> when a stored string value appears to be
    /// Unicode-corrupted: it is non-empty but contains no real letters or digits
    /// other than '?' (the replacement character from non-UTF-8 JSON serialisers).
    /// </summary>
    public static bool IsCorrupted(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return !value.Any(c => char.IsLetterOrDigit(c) && c != '?');
    }
}
