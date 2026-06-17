using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Security;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.UiConfig.Commands;

/// <summary>
/// Rolls the live configuration back to a previous revision snapshot. The current state is
/// archived first and the restore is published as a brand-new revision, so rollback is itself
/// undoable (it never destroys history).
/// </summary>
public sealed record RestoreUiConfigRevisionCommand(int Revision) : ICommand<UiConfigResponse>;

public sealed class RestoreUiConfigRevisionCommandValidator : AbstractValidator<RestoreUiConfigRevisionCommand>
{
    public RestoreUiConfigRevisionCommandValidator()
    {
        RuleFor(x => x.Revision).GreaterThan(0);
    }
}

public sealed class RestoreUiConfigRevisionCommandHandler(
    IAppDbContext db,
    ICurrentUser currentUser,
    IDateTimeProvider clock,
    ILogger<RestoreUiConfigRevisionCommandHandler> logger)
    : ICommandHandler<RestoreUiConfigRevisionCommand, UiConfigResponse>
{
    public async Task<UiConfigResponse> Handle(RestoreUiConfigRevisionCommand command, CancellationToken ct)
    {
        var snapshot = await db.UiConfigRevisions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                r => r.ConfigId == UiConfiguration.SingletonId && r.Revision == command.Revision, ct);

        if (snapshot is null)
            throw new NotFoundException($"Revision {command.Revision} not found.");

        var entity = await db.UiConfigurations
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct)
            ?? await UiConfigMapper.CreateDefaultAsync(db, ct);

        // Archive the current live state, then replay the chosen snapshot as the new revision.
        UiConfigMapper.ArchiveCurrent(db, entity);

        entity.PayloadJson = snapshot.PayloadJson;
        entity.Revision += 1;
        entity.UpdatedAtUtc = clock.UtcNow;
        entity.UpdatedBy = currentUser.UserName ?? "unknown";

        await UiConfigMapper.PruneRevisionsAsync(db, ct);

        await db.SaveChangesAsync(ct);
        logger.LogInformation(
            "UI config restored from revision {From} to new revision {To} by {User}",
            command.Revision, entity.Revision, entity.UpdatedBy);

        return UiConfigMapper.Map(entity);
    }
}
