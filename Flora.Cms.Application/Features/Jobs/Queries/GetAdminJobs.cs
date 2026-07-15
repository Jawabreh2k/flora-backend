using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Jobs.Queries;

/// <summary>Admin paged list of jobs in every status, with application counts.</summary>
public sealed record GetAdminJobsQuery(
    string? Search = null,
    JobStatus? Status = null,
    int Page = 1,
    int PageSize = 20)
    : IQuery<PaginatedResponse<AdminJobListItemDto>>;

public sealed class GetAdminJobsQueryHandler(IAppDbContext db)
    : IQueryHandler<GetAdminJobsQuery, PaginatedResponse<AdminJobListItemDto>>
{
    public async Task<PaginatedResponse<AdminJobListItemDto>> Handle(GetAdminJobsQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var q = db.JobPostings.AsNoTracking().Include(j => j.Company).AsQueryable();

        // Filter server-side so search/status apply across the whole table, not just
        // the rows that happen to land on the current page.
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(j =>
                j.Title.Contains(term) ||
                j.Department.Contains(term) ||
                j.Company.Name.Contains(term));
        }

        if (query.Status.HasValue)
            q = q.Where(j => j.Status == query.Status.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(j => j.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new AdminJobListItemDto(
                j.Id,
                j.Title,
                j.TitleAr,
                j.Department,
                j.Status,
                j.LocationType,
                j.JobType,
                j.CreatedAt,
                j.UpdatedAt,
                j.Applications.Count,
                new CompanySummaryDto(j.Company.Id, j.Company.Name, j.Company.LogoUrl, j.Company.Website)))
            .ToListAsync(ct);

        return new PaginatedResponse<AdminJobListItemDto>(items, total, page, pageSize, page * pageSize < total);
    }
}
