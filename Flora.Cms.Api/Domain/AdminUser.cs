namespace Flora.Cms.Api.Domain;

/// <summary>
/// An operator of the Admin Portal. Credentials are verified with BCrypt; the
/// portal exchanges them for a short-lived JWT used to call the protected
/// <c>/api/admin/**</c> endpoints.
/// </summary>
public class AdminUser
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Username { get; set; }

    public required string Email { get; set; }

    /// <summary>BCrypt hash of the password. The plaintext is never persisted.</summary>
    public required string PasswordHash { get; set; }

    /// <summary>Authorization role embedded in the JWT (e.g. "Admin").</summary>
    public string Role { get; set; } = Roles.Admin;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAtUtc { get; set; }
}

/// <summary>Well-known authorization role names.</summary>
public static class Roles
{
    public const string Admin = "Admin";
}
