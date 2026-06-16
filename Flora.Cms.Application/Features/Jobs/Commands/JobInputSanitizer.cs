namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>Normalises free-text list inputs (requirements / responsibilities).</summary>
internal static class JobInputSanitizer
{
    /// <summary>Trims, drops blanks, and de-duplicates (case-insensitively) the supplied items.</summary>
    public static List<string> Clean(IEnumerable<string> items)
        => items
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
}
