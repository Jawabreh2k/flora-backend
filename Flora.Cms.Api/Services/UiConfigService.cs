using System.Text.Json;
using Flora.Cms.Api.Data;
using Flora.Cms.Api.Domain;
using Flora.Cms.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Api.Services;

public class UiConfigService(FloraCmsDbContext db, ILogger<UiConfigService> logger) : IUiConfigService
{
    // Number of revision snapshots to retain per config row.
    private const int MaxRevisionHistory = 10;

    public async Task<UiConfigResponse> GetAsync(CancellationToken ct = default)
    {
        var entity = await db.UiConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct);

        if (entity is null)
            entity = await CreateDefaultAsync(ct);

        return Map(entity);
    }

    public async Task<UiConfigResponse> UpdateAsync(UiConfigPayload payload, string updatedBy, CancellationToken ct = default)
    {
        var entity = await db.UiConfigurations
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct)
            ?? await CreateDefaultAsync(ct);

        // Archive the current state before overwriting.
        db.UiConfigRevisions.Add(new UiConfigRevision
        {
            ConfigId = entity.Id,
            Revision = entity.Revision,
            PayloadJson = entity.PayloadJson,
            SavedAtUtc = entity.UpdatedAtUtc,
            SavedBy = entity.UpdatedBy,
        });

        // Merge defaults into the incoming content at write time so the stored JSON
        // is always complete. The read path also merges as a safety net.
        if (payload.Content.HasValue)
            payload.Content = ContentMerger.MergeDefaults(payload.Content.Value);

        entity.PayloadJson = JsonSerializer.Serialize(payload, JsonDefaults.Payload);
        entity.Revision += 1;
        entity.UpdatedAtUtc = DateTimeOffset.UtcNow;
        entity.UpdatedBy = updatedBy;

        // Prune old snapshots, keeping only the most recent MaxRevisionHistory entries.
        var oldRevisions = await db.UiConfigRevisions
            .Where(r => r.ConfigId == UiConfiguration.SingletonId)
            .OrderByDescending(r => r.Revision)
            .Skip(MaxRevisionHistory)
            .ToListAsync(ct);

        if (oldRevisions.Count > 0)
            db.UiConfigRevisions.RemoveRange(oldRevisions);

        await db.SaveChangesAsync(ct);
        logger.LogInformation("UI config updated to revision {Revision} by {User}", entity.Revision, updatedBy);

        return Map(entity);
    }

    public async Task<IReadOnlyList<RevisionSummary>> GetHistoryAsync(int limit = 10, CancellationToken ct = default)
    {
        return await db.UiConfigRevisions
            .AsNoTracking()
            .Where(r => r.ConfigId == UiConfiguration.SingletonId)
            .OrderByDescending(r => r.Revision)
            .Take(limit)
            .Select(r => new RevisionSummary(r.Revision, r.SavedAtUtc, r.SavedBy))
            .ToListAsync(ct);
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    private async Task<UiConfiguration> CreateDefaultAsync(CancellationToken ct)
    {
        var entity = new UiConfiguration
        {
            Id = UiConfiguration.SingletonId,
            PayloadJson = JsonSerializer.Serialize(UiConfigPayload.CreateDefault(), JsonDefaults.Payload),
        };
        db.UiConfigurations.Add(entity);
        await db.SaveChangesAsync(ct);
        return entity;
    }

    private static UiConfigResponse Map(UiConfiguration entity)
    {
        var payload = JsonSerializer.Deserialize<UiConfigPayload>(entity.PayloadJson, JsonDefaults.Payload)
                      ?? new UiConfigPayload();

        // Safety net: merge defaults on read so any row written before a schema extension
        // or corrupted by a non-UTF-8 serialiser is always returned clean.
        payload.Content = ContentMerger.MergeDefaults(payload.Content ?? default);

        return new UiConfigResponse
        {
            Payload = payload,
            Revision = entity.Revision,
            UpdatedAtUtc = entity.UpdatedAtUtc,
            UpdatedBy = entity.UpdatedBy,
        };
    }
}
