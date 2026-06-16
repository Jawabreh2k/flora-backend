using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Abstractions.Security;

/// <summary>A freshly issued access token and the instant it expires.</summary>
public readonly record struct AccessToken(string Value, DateTimeOffset ExpiresAtUtc);

/// <summary>Issues signed access tokens for authenticated admin users.</summary>
public interface ITokenService
{
    AccessToken CreateAccessToken(AdminUser user);
}

/// <summary>Hashes and verifies passwords. Implementation chooses the algorithm (BCrypt).</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

/// <summary>Ambient information about the caller making the current request.</summary>
public interface ICurrentUser
{
    /// <summary>The authenticated username/email, or <c>null</c> when anonymous.</summary>
    string? UserName { get; }
}
