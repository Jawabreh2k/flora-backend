using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Mapping;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Jobs.Queries;

/// <summary>Public detail view of a single PUBLISHED job. Returns <c>null</c> when not found.</summary>
public sealed record GetPublishedJobQuery(Guid Id) : IQuery<JobDetailDto?>;

public sealed class GetPublishedJobQueryHandler(IAppDbContext db)
    : IQueryHandler<GetPublishedJobQuery, JobDetailDto?>
{
    public async Task<JobDetailDto?> Handle(GetPublishedJobQuery query, CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == query.Id && j.Status == JobStatus.PUBLISHED, ct);

        return job is null ? null : JobMappings.ToDetail(job);
    }
}
