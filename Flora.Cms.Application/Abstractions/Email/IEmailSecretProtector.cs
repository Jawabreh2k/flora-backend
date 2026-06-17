namespace Flora.Cms.Application.Abstractions.Email;

/// <summary>
/// Encrypts/decrypts email credential secrets (SMTP password, Resend API key) so they
/// are never stored or transmitted in plaintext. Implemented in Infrastructure with
/// ASP.NET Core Data Protection.
/// </summary>
public interface IEmailSecretProtector
{
    string Protect(string plaintext);

    /// <summary>Decrypts ciphertext; returns null for null/empty input or undecryptable data.</summary>
    string? Unprotect(string? ciphertext);
}
