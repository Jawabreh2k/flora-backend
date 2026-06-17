using System.Text.Json;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Security;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Flora.Cms.Application.Features.UiConfig.Commands;

/// <summary>Replaces the live UI configuration payload, archiving the prior revision.</summary>
public sealed record UpdateUiConfigCommand(UiConfigPayload Payload) : ICommand<UiConfigResponse>;

public sealed class UpdateUiConfigCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser,
    IDateTimeProvider clock,
    ILogger<UpdateUiConfigCommandHandler> logger)
    : ICommandHandler<UpdateUiConfigCommand, UiConfigResponse>
{
    public async Task<UiConfigResponse> Handle(UpdateUiConfigCommand command, CancellationToken ct)
    {
        var updatedBy = currentUser.UserName ?? "unknown";

        var entity = await db.UiConfigurations
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct)
            ?? await UiConfigMapper.CreateDefaultAsync(db, ct);

        // Archive the current state before overwriting.
        UiConfigMapper.ArchiveCurrent(db, entity);

        var payload = command.Payload;

        // Merge defaults into the incoming content at write time so the stored JSON
        // is always complete. The read path also merges as a safety net.
        if (payload.Content.HasValue)
            payload.Content = ContentMerger.MergeDefaults(payload.Content.Value);

        entity.PayloadJson = JsonSerializer.Serialize(payload, JsonDefaults.Payload);
        entity.Revision += 1;
        entity.UpdatedAtUtc = clock.UtcNow;
        entity.UpdatedBy = updatedBy;

        await UiConfigMapper.PruneRevisionsAsync(db, ct);

        await db.SaveChangesAsync(ct);
        logger.LogInformation("UI config updated to revision {Revision} by {User}", entity.Revision, updatedBy);

        return UiConfigMapper.Map(entity);
    }
}
