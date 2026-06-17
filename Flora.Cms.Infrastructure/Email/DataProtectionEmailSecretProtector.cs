using System.Security.Cryptography;
using Flora.Cms.Application.Abstractions.Email;
using Microsoft.AspNetCore.DataProtection;

namespace Flora.Cms.Infrastructure.Email;

/// <summary>
/// Encrypts email secrets at rest with ASP.NET Core Data Protection. The purpose string
/// scopes the keys; rotating the key ring renders old ciphertext undecryptable, in which
/// case <see cref="Unprotect"/> returns null and the admin simply re-enters the secret.
/// </summary>
public sealed class DataProtectionEmailSecretProtector : IEmailSecretProtector
{
    private readonly IDataProtector _protector;

    public DataProtectionEmailSecretProtector(IDataProtectionProvider provider)
        => _protector = provider.CreateProtector("Flora.Cms.EmailSettings.Secrets.v1");

    public string Protect(string plaintext) => _protector.Protect(plaintext);

    public string? Unprotect(string? ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext)) return null;
        try
        {
            return _protector.Unprotect(ciphertext);
        }
        catch (CryptographicException)
        {
            return null;
        }
    }
}
