using Flora.Cms.Api.Data;
using Flora.Cms.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Api.Services;

public class AuthService(FloraCmsDbContext db, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await db.AdminUsers
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive, ct);

        // Verify even when the user is missing to keep timing roughly constant and
        // avoid leaking whether the username exists.
        var hash = user?.PasswordHash ?? "$2a$11$invalidinvalidinvalidinvalidinvalidinvalidinvalidinv";
        var passwordOk = BCrypt.Net.BCrypt.Verify(request.Password, hash);

        if (user is null || !passwordOk)
        {
            return null;
        }

        user.LastLoginAtUtc = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);

        var (token, expiresAt) = tokenService.CreateAccessToken(user);
        return new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresAtUtc = expiresAt,
            Username = user.Username,
            Role = user.Role,
        };
    }
}
