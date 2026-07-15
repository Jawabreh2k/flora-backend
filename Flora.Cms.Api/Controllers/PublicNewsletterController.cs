using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Newsletter.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/newsletter")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicNewsletterController(ISender sender) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("newsletter")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Subscribe(
        [FromBody] SubscribeNewsletterRequest request,
        CancellationToken ct)
    {
        var id = await sender.Send(
            new SubscribeToNewsletterCommand(request.Email, request.Locale ?? "en"),
            ct);

        return Created(string.Empty, new { id });
    }
}
