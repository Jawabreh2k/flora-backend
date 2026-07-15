namespace Flora.Cms.Application.Features.Newsletter.Contracts;

/// <summary>A newsletter subscriber as shown in the admin list.</summary>
public record NewsletterSubscriptionDto(
    Guid Id,
    string Email,
    string Locale,
    DateTimeOffset CreatedAtUtc);
