using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Applications.Commands;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Application.Features.Applications.Queries;
using Flora.Cms.Application.Features.Jobs.Commands;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Queries;
using Flora.Cms.Domain.Common;
using Flora.Cms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/admin/jobs")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminJobsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<AdminJobListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<AdminJobListItemDto>>> List(
        [FromQuery] string? search,
        [FromQuery] JobStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetAdminJobsQuery(search, status, page, pageSize), ct));

    [HttpGet("companies")]
    [ProducesResponseType(typeof(IReadOnlyList<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompanyDto>>> Companies(CancellationToken ct)
        => Ok(await sender.Send(new GetCompaniesQuery(), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var job = await sender.Send(new GetAdminJobQuery(id), ct);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobDetailDto>> Create([FromBody] UpsertJobRequest request, CancellationToken ct)
    {
        var job = await sender.Send(new CreateJobCommand(ToWriteModel(request)), ct);
        return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Update(
        Guid id,
        [FromBody] UpsertJobRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateJobCommand(id, ToWriteModel(request)), ct));

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateJobStatusRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateJobStatusCommand(id, request.Status), ct));

    [HttpGet("{id:guid}/applications")]
    [ProducesResponseType(typeof(PaginatedResponse<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResponse<ApplicationDto>>> Applications(
        Guid id,
        [FromQuery] string? search,
        [FromQuery] ApplicationStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await sender.Send(new GetApplicationsForJobQuery(id, search, status, page, pageSize), ct));

    private static JobWriteModel ToWriteModel(UpsertJobRequest r) => new(
        r.CompanyId,
        r.Title,
        r.Department,
        r.LocationType,
        r.LocationText,
        r.JobType,
        r.Description,
        r.Requirements ?? [],
        r.Responsibilities ?? [],
        r.SalaryRange,
        r.Status,
        r.ClosingDate);
}

[ApiController]
[Route("api/admin/applications")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminApplicationsController(ISender sender) : ControllerBase
{
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateApplicationStatusRequest request,
        CancellationToken ct)
        => Ok(await sender.Send(new UpdateApplicationStatusCommand(id, request.Status), ct));
}
