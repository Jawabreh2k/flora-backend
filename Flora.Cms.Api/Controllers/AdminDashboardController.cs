using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Dashboard.Queries;
using Flora.Cms.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>Aggregated business counters backing the admin portal's Overview page.</summary>
[ApiController]
[Route("api/admin/dashboard")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminDashboardController(ISender sender) : ControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> Summary(CancellationToken ct)
        => Ok(await sender.Send(new GetDashboardSummaryQuery(), ct));
}
