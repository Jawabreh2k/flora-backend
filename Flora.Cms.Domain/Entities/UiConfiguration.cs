namespace Flora.Cms.Domain.Entities;

/// <summary>
/// Singleton aggregate that holds the entire live UI configuration for the
/// public Flora Group website (theme colours, typography, branding, section copy).
///
/// Design decision (per spec): the configurable payload is persisted as a single
/// serialized JSON document in <see cref="PayloadJson"/> (nvarchar(max)). This lets
/// the Admin Portal add new colour/branding fields over time WITHOUT requiring an
/// EF Core migration — the schema never changes, only the JSON shape does.
///
/// The row is treated as a singleton: exactly one record identified by
/// <see cref="SingletonId"/> ever exists.
/// </summary>
public class UiConfiguration
{
    /// <summary>The fixed primary-key value for the one-and-only configuration row.</summary>
    public const int SingletonId = 1;

    public int Id { get; set; } = SingletonId;

    /// <summary>
    /// The full UI configuration serialized as JSON. Stored as nvarchar(max).
    /// Shape mirrors <c>UiConfig.Contracts.UiConfigPayload</c>. Never null.
    /// </summary>
    public string PayloadJson { get; set; } = "{}";

    /// <summary>
    /// Monotonic revision counter, incremented on every successful update. Surfaced
    /// to clients as an ETag so the public site can cache aggressively and the Admin
    /// Portal can perform optimistic-concurrency checks on top of <see cref="RowVersion"/>.
    /// </summary>
    public int Revision { get; set; } = 1;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Username / email of the admin who last saved the configuration.</summary>
    public string? UpdatedBy { get; set; }

    /// <summary>SQL Server rowversion for optimistic concurrency control.</summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
