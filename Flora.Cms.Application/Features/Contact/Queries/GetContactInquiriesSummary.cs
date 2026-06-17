using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Features.Contact.Contracts;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Contact.Queries;

/// <summary>Lightweight inbox counters (total + unread) for the nav badge and dashboard.</summary>
public sealed record GetContactInquiriesSummaryQuery : IQuery<ContactInquirySummaryDto>;

public sealed class GetContactInquiriesSummaryQueryHandler(IAppDbContext db)
    : IQueryHandler<GetContactInquiriesSummaryQuery, ContactInquirySummaryDto>
{
    public async Task<ContactInquirySummaryDto> Handle(GetContactInquiriesSummaryQuery query, CancellationToken ct)
    {
        var total = await db.ContactInquiries.AsNoTracking().CountAsync(ct);
        var unread = await db.ContactInquiries.AsNoTracking()
            .CountAsync(i => i.Status == ContactInquiryStatus.NEW, ct);

        return new ContactInquirySummaryDto(total, unread);
    }
}
