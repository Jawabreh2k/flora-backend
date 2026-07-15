using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Newsletter.Contracts;
using Flora.Cms.Application.Features.Newsletter.Queries;
using Flora.Cms.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>Admin view of newsletter signups captured from the public site footer.</summary>
[ApiController]
[Route("api/admin/newsletter")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminNewsletterController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<NewsletterSubscriptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<NewsletterSubscriptionDto>>> List(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetNewsletterSubscriptionsQuery(search, page, pageSize), ct));
}
