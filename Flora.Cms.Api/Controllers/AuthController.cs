using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Auth.Contracts;
using Flora.Cms.Application.Features.Auth.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Flora.Cms.Api.Controllers;

/// <summary>Authentication for the Admin Portal.</summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
[Produces("application/json")]
public class AuthController(ISender sender) : ControllerBase
{
    /// <summary>Exchanges admin credentials for a short-lived JWT access token.</summary>
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new LoginCommand(request.Username, request.Password), ct);
        if (result is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Invalid credentials",
                Status = StatusCodes.Status401Unauthorized,
            });
        }

        return Ok(result);
    }
}
