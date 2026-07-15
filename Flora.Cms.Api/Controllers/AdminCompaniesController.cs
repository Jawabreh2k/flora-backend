using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Features.Companies.Commands;
using Flora.Cms.Application.Features.Companies.Queries;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Queries;
using Flora.Cms.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>
/// Admin CRUD for companies that job postings are filed under. The public site
/// never sees this directly — a job's company is exposed only through
/// JobDetailDto/JobListItemDto's embedded CompanyDto.
/// </summary>
[ApiController]
[Route("api/admin/companies")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminCompaniesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompanyDto>>> List(CancellationToken ct)
        => Ok(await sender.Send(new GetCompaniesQuery(), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> Get(Guid id, CancellationToken ct)
    {
        var company = await sender.Send(new GetCompanyByIdQuery(id), ct);
        return company is null ? NotFound() : Ok(company);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CompanyDto>> Create([FromBody] UpsertCompanyRequest request, CancellationToken ct)
    {
        var created = await sender.Send(
            new CreateCompanyCommand(request.Name, request.LogoUrl, request.Website, request.Description), ct);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CompanyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CompanyDto>> Update(
        Guid id,
        [FromBody] UpsertCompanyRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(
            new UpdateCompanyCommand(id, request.Name, request.LogoUrl, request.Website, request.Description), ct));
}
