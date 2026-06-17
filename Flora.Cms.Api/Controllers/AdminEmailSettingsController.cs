using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Email.Commands;
using Flora.Cms.Application.Features.Email.Contracts;
using Flora.Cms.Application.Features.Email.Queries;
using Flora.Cms.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>
/// Admin-only email delivery configuration. Lives outside the public UI config payload
/// because it carries secrets; the API never returns the secret values themselves.
/// </summary>
[ApiController]
[Route("api/admin/email-settings")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminEmailSettingsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EmailSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EmailSettingsDto>> Get(CancellationToken ct)
        => Ok(await sender.Send(new GetEmailSettingsQuery(), ct));

    [HttpPut]
    [ProducesResponseType(typeof(EmailSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EmailSettingsDto>> Update(
        [FromBody] UpdateEmailSettingsRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateEmailSettingsCommand(new UpdateEmailSettingsInput(
            request.Provider,
            request.FromAddress,
            request.SmtpHost,
            request.SmtpPort,
            request.SmtpUseSsl,
            request.SmtpUser,
            request.SmtpPassword,
            request.ResendApiKey)), ct));

    [HttpPost("test")]
    [ProducesResponseType(typeof(TestEmailResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TestEmailResultDto>> SendTest(
        [FromBody] SendTestEmailRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new SendTestEmailCommand(request.To), ct));
}
