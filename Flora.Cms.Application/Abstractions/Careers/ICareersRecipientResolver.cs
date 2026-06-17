namespace Flora.Cms.Application.Abstractions.Careers;

/// <summary>
/// Resolves the HR inbox that receives new-application alerts, from published CMS config
/// (the careers HR email), never from the client.
/// </summary>
public interface ICareersRecipientResolver
{
    Task<string> ResolveAsync(string locale, CancellationToken ct = default);
}
