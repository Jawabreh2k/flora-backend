using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Contact.Commands;
using Flora.Cms.Application.Features.Contact.Contracts;
using Flora.Cms.Application.Features.Contact.Queries;
using Flora.Cms.Domain.Common;
using Flora.Cms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>Admin inbox for contact inquiries submitted from the public site.</summary>
[ApiController]
[Route("api/admin/inquiries")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminContactController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ContactInquiryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<ContactInquiryDto>>> List(
        [FromQuery] string? search,
        [FromQuery] ContactInquiryStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetContactInquiriesQuery(search, status, page, pageSize), ct));

    [HttpGet("summary")]
    [ProducesResponseType(typeof(ContactInquirySummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ContactInquirySummaryDto>> Summary(CancellationToken ct)
        => Ok(await sender.Send(new GetContactInquiriesSummaryQuery(), ct));

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ContactInquiryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContactInquiryDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateContactInquiryStatusRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateContactInquiryStatusCommand(id, request.Status), ct));
}
