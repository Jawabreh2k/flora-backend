using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Applications.Queries;

/// <summary>Admin paged list of the applications submitted to a given job posting.</summary>
public sealed record GetApplicationsForJobQuery(
    Guid JobPostingId,
    string? Search = null,
    ApplicationStatus? Status = null,
    int Page = 1,
    int PageSize = 20)
    : IQuery<PaginatedResponse<ApplicationDto>>;

public sealed class GetApplicationsForJobQueryHandler(IAppDbContext db)
    : IQueryHandler<GetApplicationsForJobQuery, PaginatedResponse<ApplicationDto>>
{
    public async Task<PaginatedResponse<ApplicationDto>> Handle(GetApplicationsForJobQuery query, CancellationToken ct)
    {
        var jobExists = await db.JobPostings.AnyAsync(j => j.Id == query.JobPostingId, ct);
        if (!jobExists)
            throw new NotFoundException("Job posting not found.");

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Applications
            .AsNoTracking()
            .Where(a => a.JobPostingId == query.JobPostingId);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(a =>
                a.CandidateName.ToLower().Contains(term) ||
                a.CandidateEmail.ToLower().Contains(term));
        }

        if (query.Status.HasValue)
            q = q.Where(a => a.Status == query.Status.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ApplicationDto(
                a.Id,
                a.JobPostingId,
                a.CandidateName,
                a.CandidateEmail,
                a.CandidatePhone,
                a.ResumeUrl,
                a.CoverLetter,
                a.Status,
                a.AppliedAt))
            .ToListAsync(ct);

        return new PaginatedResponse<ApplicationDto>(items, total, page, pageSize, page * pageSize < total);
    }
}
