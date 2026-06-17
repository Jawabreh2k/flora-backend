using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Contact.Contracts;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Contact.Queries;

/// <summary>Admin paged list of contact inquiries, newest first, with optional search/status filters.</summary>
public sealed record GetContactInquiriesQuery(
    string? Search = null,
    ContactInquiryStatus? Status = null,
    int Page = 1,
    int PageSize = 20)
    : IQuery<PaginatedResponse<ContactInquiryDto>>;

public sealed class GetContactInquiriesQueryHandler(IAppDbContext db)
    : IQueryHandler<GetContactInquiriesQuery, PaginatedResponse<ContactInquiryDto>>
{
    public async Task<PaginatedResponse<ContactInquiryDto>> Handle(GetContactInquiriesQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.ContactInquiries.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim().ToLower();
            q = q.Where(i =>
                i.Name.ToLower().Contains(term) ||
                i.Email.ToLower().Contains(term) ||
                i.Subject.ToLower().Contains(term) ||
                (i.Company != null && i.Company.ToLower().Contains(term)));
        }

        if (query.Status.HasValue)
            q = q.Where(i => i.Status == query.Status.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(i => new ContactInquiryDto(
                i.Id,
                i.Name,
                i.Email,
                i.Company,
                i.Subject,
                i.Message,
                i.Locale,
                i.Status,
                i.CreatedAtUtc))
            .ToListAsync(ct);

        return new PaginatedResponse<ContactInquiryDto>(items, total, page, pageSize, page * pageSize < total);
    }
}
