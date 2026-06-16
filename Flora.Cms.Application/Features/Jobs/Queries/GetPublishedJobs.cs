using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Jobs.Queries;

/// <summary>Public, filterable, paged list of PUBLISHED job postings.</summary>
public sealed record GetPublishedJobsQuery(
    string? Search = null,
    string? Department = null,
    LocationType? LocationType = null,
    JobType? JobType = null,
    int Page = 1,
    int PageSize = 12) : IQuery<PaginatedResponse<JobListItemDto>>;

public sealed class GetPublishedJobsQueryHandler(IAppDbContext db)
    : IQueryHandler<GetPublishedJobsQuery, PaginatedResponse<JobListItemDto>>
{
    public async Task<PaginatedResponse<JobListItemDto>> Handle(GetPublishedJobsQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var q = db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .Where(j => j.Status == JobStatus.PUBLISHED);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(j =>
                j.Title.Contains(term) ||
                j.Company.Name.Contains(term) ||
                j.Department.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Department))
            q = q.Where(j => j.Department == query.Department);

        if (query.LocationType.HasValue)
            q = q.Where(j => j.LocationType == query.LocationType.Value);

        if (query.JobType.HasValue)
            q = q.Where(j => j.JobType == query.JobType.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobListItemDto(
                j.Id,
                j.Title,
                j.Department,
                j.LocationType,
                j.LocationText,
                j.JobType,
                j.SalaryRange,
                j.ClosingDate,
                j.CreatedAt,
                new CompanySummaryDto(j.Company.Id, j.Company.Name, j.Company.LogoUrl, j.Company.Website)))
            .ToListAsync(ct);

        return new PaginatedResponse<JobListItemDto>(items, total, page, pageSize, page * pageSize < total);
    }
}
