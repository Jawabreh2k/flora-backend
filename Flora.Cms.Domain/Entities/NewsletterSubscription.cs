namespace Flora.Cms.Domain.Entities;

/// <summary>
/// A newsletter signup captured from the public site footer. Emails are stored
/// normalized (trimmed, lowercased) so the unique index is collation-independent.
/// </summary>
public class NewsletterSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Email { get; set; }

    /// <summary>UI locale the visitor subscribed from ("en"/"ar") — lets marketing segment sends.</summary>
    public string Locale { get; set; } = "en";

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
