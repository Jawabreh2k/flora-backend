using Flora.Cms.Application.Features.Contact.Contracts;
using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Features.Contact.Mapping;

internal static class ContactInquiryMappings
{
    public static ContactInquiryDto ToDto(ContactInquiry i) => new(
        i.Id,
        i.Name,
        i.Email,
        i.Company,
        i.Subject,
        i.Message,
        i.Locale,
        i.Status,
        i.CreatedAtUtc);
}
