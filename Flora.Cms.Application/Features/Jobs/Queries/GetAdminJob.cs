using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Jobs.Queries;

/// <summary>Admin detail view of any job regardless of status. <c>null</c> when not found.</summary>
public sealed record GetAdminJobQuery(Guid Id) : IQuery<JobDetailDto?>;

public sealed class GetAdminJobQueryHandler(IAppDbContext db)
    : IQueryHandler<GetAdminJobQuery, JobDetailDto?>
{
    public async Task<JobDetailDto?> Handle(GetAdminJobQuery query, CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == query.Id, ct);

        return job is null ? null : JobMappings.ToDetail(job);
    }
}
