using Flora.Cms.Api.Dtos;

namespace Flora.Cms.Api.Services;

public interface IUiConfigService
{
    /// <summary>Reads the current live UI configuration (creating defaults if none exist).</summary>
    Task<UiConfigResponse> GetAsync(CancellationToken ct = default);

    /// <summary>
    /// Replaces the configurable payload with <paramref name="payload"/>, bumping the
    /// revision, archiving the previous state, and recording who made the change.
    /// </summary>
    Task<UiConfigResponse> UpdateAsync(UiConfigPayload payload, string updatedBy, CancellationToken ct = default);

    /// <summary>
    /// Returns the last <paramref name="limit"/> revision snapshots, newest first.
    /// Used by the Admin Portal to show history and offer rollback.
    /// </summary>
    Task<IReadOnlyList<RevisionSummary>> GetHistoryAsync(int limit = 10, CancellationToken ct = default);
}

/// <summary>Lightweight history entry returned by the Admin Portal history endpoint.</summary>
public record RevisionSummary(int Revision, DateTimeOffset SavedAtUtc, string? SavedBy);
