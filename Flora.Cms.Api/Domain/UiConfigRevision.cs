namespace Flora.Cms.Api.Domain;

/// <summary>
/// Point-in-time snapshot of <see cref="UiConfiguration"/>, written automatically
/// each time the live config is updated. The last 10 snapshots are retained so the
/// Admin Portal can display history and perform a rollback without a full restore.
/// </summary>
public class UiConfigRevision
{
    public int Id { get; set; }

    /// <summary>FK to the singleton UiConfiguration row (always <see cref="UiConfiguration.SingletonId"/>).</summary>
    public int ConfigId { get; set; } = UiConfiguration.SingletonId;

    /// <summary>The revision number this snapshot was taken from (before the increment).</summary>
    public int Revision { get; set; }

    /// <summary>Full payload JSON at this point in time — nvarchar(max), same shape as UiConfiguration.PayloadJson.</summary>
    public string PayloadJson { get; set; } = "{}";

    public DateTimeOffset SavedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Admin username who triggered the save.</summary>
    public string? SavedBy { get; set; }
}
