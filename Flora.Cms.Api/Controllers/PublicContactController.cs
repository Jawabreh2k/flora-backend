using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Contact.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/contact")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicContactController(ISender sender) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("contact")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit(
        [FromBody] SubmitContactRequest request,
        CancellationToken ct)
    {
        var id = await sender.Send(
            new SubmitContactCommand(
                request.Name,
                request.Email,
                request.Company,
                request.Subject,
                request.Message,
                request.Locale ?? "en"),
            ct);

        return Created(string.Empty, new { id });
    }
}

public sealed class SubmitContactRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Locale { get; set; }
}
