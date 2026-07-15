using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Dashboard.Queries;

/// <summary>Business-facing counters for the admin Overview page — one call, one screen.</summary>
public sealed record GetDashboardSummaryQuery : IQuery<DashboardSummaryDto>;

public record DashboardSummaryDto(
    DashboardJobStatsDto Jobs,
    DashboardApplicationStatsDto Applications,
    DashboardInquiryStatsDto Inquiries,
    DashboardNewsletterStatsDto Newsletter,
    IReadOnlyList<DashboardTopJobDto> TopJobs);

public record DashboardJobStatsDto(int Published, int Draft, int Archived);

public record DashboardApplicationStatsDto(int Total, int AwaitingReview, int LastSevenDays);

public record DashboardInquiryStatsDto(int Total, int Unread);

public record DashboardNewsletterStatsDto(int Total, int LastThirtyDays);

/// <summary>A job ranked by how many applications it has attracted.</summary>
public record DashboardTopJobDto(Guid JobId, string Title, string Department, int ApplicationCount);

public sealed class GetDashboardSummaryQueryHandler(IAppDbContext db, IDateTimeProvider clock)
    : IQueryHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery query, CancellationToken ct)
    {
        var now = clock.UtcNow;
        var sevenDaysAgo = now.AddDays(-7);
        var thirtyDaysAgo = now.AddDays(-30);

        var jobsByStatus = await db.JobPostings.AsNoTracking()
            .GroupBy(j => j.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, ct);

        var jobs = new DashboardJobStatsDto(
            jobsByStatus.GetValueOrDefault(JobStatus.PUBLISHED),
            jobsByStatus.GetValueOrDefault(JobStatus.DRAFT),
            jobsByStatus.GetValueOrDefault(JobStatus.ARCHIVED));

        var applications = new DashboardApplicationStatsDto(
            await db.Applications.AsNoTracking().CountAsync(ct),
            await db.Applications.AsNoTracking().CountAsync(a => a.Status == ApplicationStatus.APPLIED, ct),
            await db.Applications.AsNoTracking().CountAsync(a => a.AppliedAt >= sevenDaysAgo, ct));

        var inquiries = new DashboardInquiryStatsDto(
            await db.ContactInquiries.AsNoTracking().CountAsync(ct),
            await db.ContactInquiries.AsNoTracking().CountAsync(i => i.Status == ContactInquiryStatus.NEW, ct));

        var newsletter = new DashboardNewsletterStatsDto(
            await db.NewsletterSubscriptions.AsNoTracking().CountAsync(ct),
            await db.NewsletterSubscriptions.AsNoTracking().CountAsync(s => s.CreatedAtUtc >= thirtyDaysAgo, ct));

        // Published jobs ranked by application volume — surfaces both what's
        // working and (count == 0) what's getting no traction.
        //
        // Ordering has to happen on the anonymous projection, not the final
        // DashboardTopJobDto: EF Core can't translate an OrderBy through a
        // record constructor when the ordered member is itself a correlated
        // subquery count, so the DTO mapping is a second, in-memory-shape-only
        // projection applied after Take(5).
        var topJobs = (await db.JobPostings.AsNoTracking()
            .Where(j => j.Status == JobStatus.PUBLISHED)
            .Select(j => new { j.Id, j.Title, j.Department, ApplicationCount = j.Applications.Count })
            .OrderByDescending(j => j.ApplicationCount)
            .ThenBy(j => j.Title)
            .Take(5)
            .ToListAsync(ct))
            .Select(j => new DashboardTopJobDto(j.Id, j.Title, j.Department, j.ApplicationCount))
            .ToList();

        return new DashboardSummaryDto(jobs, applications, inquiries, newsletter, topJobs);
    }
}
