namespace Flora.Cms.Application.Features.UiConfig.Contracts;

/// <summary>
/// What the API returns for both the public GET and the admin PUT. Wraps the
/// configurable <see cref="Payload"/> with server-managed metadata (revision &amp;
/// last-updated) that clients use for caching and display.
/// </summary>
public class UiConfigResponse
{
    public required UiConfigPayload Payload { get; set; }
    public int Revision { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}

/// <summary>Lightweight history entry returned by the Admin Portal history endpoint.</summary>
public record RevisionSummary(int Revision, DateTimeOffset SavedAtUtc, string? SavedBy);
