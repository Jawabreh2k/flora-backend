namespace Flora.Cms.Application.Features.Auth.Contracts;

/// <summary>The successful result of a login: a bearer token plus who it belongs to.</summary>
public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; } = "Bearer";
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
}
