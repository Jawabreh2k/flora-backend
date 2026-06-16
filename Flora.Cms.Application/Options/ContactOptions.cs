namespace Flora.Cms.Application.Options;

/// <summary>
/// Server-side contact delivery settings (secrets stay in configuration / user secrets).
/// </summary>
public sealed class ContactOptions
{
    public const string SectionName = "Contact";

    /// <summary>Fallback inbox when CMS <c>formRecipientEmail</c> is empty.</summary>
    public string RecipientEmail { get; set; } = "info@floragroup.net";

    public string FromAddress { get; set; } = "Flora Group <noreply@floragroup.net>";

    public string? ResendApiKey { get; set; }

    public string? SmtpHost { get; set; }
    public int SmtpPort { get; set; } = 587;
    public bool SmtpUseSsl { get; set; } = true;
    public string? SmtpUser { get; set; }
    public string? SmtpPassword { get; set; }
}
