using Flora.Cms.Api.Dtos;

namespace Flora.Cms.Api.Services;

public interface IAuthService
{
    /// <summary>
    /// Validates credentials and issues a JWT. Returns null when authentication fails
    /// (caller should respond 401 without leaking which part was wrong).
    /// </summary>
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
}
