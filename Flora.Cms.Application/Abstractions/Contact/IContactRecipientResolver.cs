namespace Flora.Cms.Application.Abstractions.Contact;

/// <summary>
/// Resolves the contact-form delivery inbox from published CMS config (never from the client).
/// </summary>
public interface IContactRecipientResolver
{
    Task<string> ResolveAsync(string locale, CancellationToken ct = default);
}
