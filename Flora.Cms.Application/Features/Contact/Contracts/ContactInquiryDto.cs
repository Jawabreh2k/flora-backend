using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Contact.Contracts;

/// <summary>A contact inquiry as shown in the admin inbox.</summary>
public record ContactInquiryDto(
    Guid Id,
    string Name,
    string Email,
    string? Company,
    string Subject,
    string Message,
    string Locale,
    ContactInquiryStatus Status,
    DateTimeOffset CreatedAtUtc);

/// <summary>Inbox counters used for the nav badge and dashboard.</summary>
public record ContactInquirySummaryDto(int Total, int Unread);
