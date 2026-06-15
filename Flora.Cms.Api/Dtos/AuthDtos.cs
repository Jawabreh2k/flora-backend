using System.ComponentModel.DataAnnotations;

namespace Flora.Cms.Api.Dtos;

public class LoginRequest
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class AuthResponse
{
    public required string AccessToken { get; set; }
    public required string TokenType { get; set; } = "Bearer";
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
}
