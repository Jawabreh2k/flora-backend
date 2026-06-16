using Flora.Cms.Application.Abstractions.Security;

namespace Flora.Cms.Infrastructure.Security;

/// <summary>BCrypt-backed <see cref="IPasswordHasher"/> (work factor 11).</summary>
public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);

    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
