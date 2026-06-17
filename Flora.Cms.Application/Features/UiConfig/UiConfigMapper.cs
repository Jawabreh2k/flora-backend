using System.Text.Json;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.UiConfig;

/// <summary>Shared (de)serialization + self-healing helpers for the UI config aggregate.</summary>
internal static class UiConfigMapper
{
    /// <summary>How many revision snapshots to retain per config row.</summary>
    public const int MaxRevisionHistory = 10;

    /// <summary>Snapshots the current live state into the revision history before it's overwritten.</summary>
    public static void ArchiveCurrent(IAppDbContext db, UiConfiguration entity)
    {
        db.UiConfigRevisions.Add(new UiConfigRevision
        {
            ConfigId = entity.Id,
            Revision = entity.Revision,
            PayloadJson = entity.PayloadJson,
            SavedAtUtc = entity.UpdatedAtUtc,
            SavedBy = entity.UpdatedBy,
        });
    }

    /// <summary>Trims the revision history to the most recent <see cref="MaxRevisionHistory"/> entries.</summary>
    public static async Task PruneRevisionsAsync(IAppDbContext db, CancellationToken ct)
    {
        var stale = await db.UiConfigRevisions
            .Where(r => r.ConfigId == UiConfiguration.SingletonId)
            .OrderByDescending(r => r.Revision)
            .Skip(MaxRevisionHistory)
            .ToListAsync(ct);

        if (stale.Count > 0)
            db.UiConfigRevisions.RemoveRange(stale);
    }

    /// <summary>Creates and persists the singleton config row seeded with brand defaults.</summary>
    public static async Task<UiConfiguration> CreateDefaultAsync(IAppDbContext db, CancellationToken ct)
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

    public static UiConfigResponse Map(UiConfiguration entity)
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
