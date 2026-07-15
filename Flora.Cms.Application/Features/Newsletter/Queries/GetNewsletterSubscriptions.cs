using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Newsletter.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Newsletter.Queries;

/// <summary>Admin paged list of newsletter subscribers, newest first, with optional email search.</summary>
public sealed record GetNewsletterSubscriptionsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20)
    : IQuery<PaginatedResponse<NewsletterSubscriptionDto>>;

public sealed class GetNewsletterSubscriptionsQueryHandler(IAppDbContext db)
    : IQueryHandler<GetNewsletterSubscriptionsQuery, PaginatedResponse<NewsletterSubscriptionDto>>
{
    public async Task<PaginatedResponse<NewsletterSubscriptionDto>> Handle(
        GetNewsletterSubscriptionsQuery query,
        CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        // Upper bound is generous (vs. 100 elsewhere) so the admin CSV export can
        // pull the full list in a handful of requests.
        var pageSize = Math.Clamp(query.PageSize, 1, 500);

        var q = db.NewsletterSubscriptions.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(s => s.Email.Contains(term));
        }

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(s => s.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new NewsletterSubscriptionDto(s.Id, s.Email, s.Locale, s.CreatedAtUtc))
            .ToListAsync(ct);

        return new PaginatedResponse<NewsletterSubscriptionDto>(items, total, page, pageSize, page * pageSize < total);
    }
}
