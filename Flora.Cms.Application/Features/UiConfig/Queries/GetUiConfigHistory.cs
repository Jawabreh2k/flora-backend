using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.UiConfig.Queries;

/// <summary>Returns the most recent revision snapshots (newest first) for the history view.</summary>
public sealed record GetUiConfigHistoryQuery(int Limit = 10) : IQuery<IReadOnlyList<RevisionSummary>>;

public sealed class GetUiConfigHistoryQueryHandler(IAppDbContext db)
    : IQueryHandler<GetUiConfigHistoryQuery, IReadOnlyList<RevisionSummary>>
{
    public async Task<IReadOnlyList<RevisionSummary>> Handle(GetUiConfigHistoryQuery query, CancellationToken ct)
        => await db.UiConfigRevisions
            .AsNoTracking()
            .Where(r => r.ConfigId == UiConfiguration.SingletonId)
            .OrderByDescending(r => r.Revision)
            .Take(query.Limit)
            .Select(r => new RevisionSummary(r.Revision, r.SavedAtUtc, r.SavedBy))
            .ToListAsync(ct);
}
