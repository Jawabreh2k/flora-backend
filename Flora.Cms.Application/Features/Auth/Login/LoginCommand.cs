using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Security;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Auth.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Application.Features.Auth.Login;

/// <summary>
/// Exchanges admin credentials for a short-lived JWT. Returns <c>null</c> on failure
/// so the caller can respond 401 without leaking which part of the credentials was wrong.
/// </summary>
public sealed record LoginCommand(string Username, string Password) : ICommand<AuthResponse?>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler(
    IAppDbContext db,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IDateTimeProvider clock)
    : ICommandHandler<LoginCommand, AuthResponse?>
{
    // A valid-shaped but unmatchable BCrypt hash. Verifying against it when the user
    // is missing keeps response timing roughly constant, avoiding username enumeration.
    private const string DummyHash = "$2a$11$invalidinvalidinvalidinvalidinvalidinvalidinvalidinv";

    public async Task<AuthResponse?> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await db.AdminUsers
            .FirstOrDefaultAsync(u => u.Username == command.Username && u.IsActive, ct);

        var hash = user?.PasswordHash ?? DummyHash;
        var passwordOk = passwordHasher.Verify(command.Password, hash);

        if (user is null || !passwordOk)
            return null;

        user.LastLoginAtUtc = clock.UtcNow;
        await db.SaveChangesAsync(ct);

        var token = tokenService.CreateAccessToken(user);
        return new AuthResponse
        {
            AccessToken = token.Value,
            TokenType = "Bearer",
            ExpiresAtUtc = token.ExpiresAtUtc,
            Username = user.Username,
            Role = user.Role,
        };
    }
}
