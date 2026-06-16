using System.Text.Json;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Features.UiConfig;

/// <summary>Shared (de)serialization + self-healing helpers for the UI config aggregate.</summary>
internal static class UiConfigMapper
{
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
