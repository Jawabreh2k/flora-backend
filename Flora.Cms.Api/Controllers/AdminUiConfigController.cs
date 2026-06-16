using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.UiConfig.Commands;
using Flora.Cms.Application.Features.UiConfig.Contracts;
using Flora.Cms.Application.Features.UiConfig.Queries;
using Flora.Cms.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>
/// JWT-protected endpoint used by the Admin Portal to read and update the live UI
/// configuration. Requires an authenticated user in the Admin role.
/// </summary>
[ApiController]
[Route("api/admin/ui/config")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminUiConfigController(ISender sender) : ControllerBase
{
    /// <summary>Returns the current configuration for editing in the portal.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(UiConfigResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UiConfigResponse>> Get(CancellationToken ct)
        => Ok(await sender.Send(new GetUiConfigQuery(), ct));

    /// <summary>Returns the last 10 revision snapshots, newest first.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IReadOnlyList<RevisionSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<RevisionSummary>>> GetHistory(CancellationToken ct)
        => Ok(await sender.Send(new GetUiConfigHistoryQuery(), ct));

    /// <summary>Replaces the live UI configuration payload.</summary>
    [HttpPut]
    [ProducesResponseType(typeof(UiConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<UiConfigResponse>> Update(
        [FromBody] UiConfigPayload payload,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateUiConfigCommand(payload), ct));
}
