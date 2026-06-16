using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.UiConfig.Queries;

/// <summary>Reads the current live UI configuration, self-healing to defaults if absent.</summary>
public sealed record GetUiConfigQuery : IQuery<UiConfigResponse>;

public sealed class GetUiConfigQueryHandler(IAppDbContext db)
    : IQueryHandler<GetUiConfigQuery, UiConfigResponse>
{
    public async Task<UiConfigResponse> Handle(GetUiConfigQuery query, CancellationToken ct)
    {
        var entity = await db.UiConfigurations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == UiConfiguration.SingletonId, ct)
            ?? await UiConfigMapper.CreateDefaultAsync(db, ct);

        return UiConfigMapper.Map(entity);
    }
}
