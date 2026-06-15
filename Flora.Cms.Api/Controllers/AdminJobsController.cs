using Flora.Cms.Api.Domain;
using Flora.Cms.Api.Dtos;
using Flora.Cms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/admin/jobs")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminJobsController(IJobService jobs) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<AdminJobListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<AdminJobListItemDto>>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await jobs.GetAdminJobsAsync(page, pageSize, ct));

    [HttpGet("companies")]
    [ProducesResponseType(typeof(IReadOnlyList<CompanyDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CompanyDto>>> Companies(CancellationToken ct)
        => Ok(await jobs.GetCompaniesAsync(ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var job = await jobs.GetAdminJobAsync(id, ct);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JobDetailDto>> Create(
        [FromBody] UpsertJobRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var job = await jobs.CreateJobAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = job.Id }, job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Update(
        Guid id,
        [FromBody] UpsertJobRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var job = await jobs.UpdateJobAsync(id, request, ct);
            return job is null ? NotFound() : Ok(job);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateJobStatusRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var job = await jobs.UpdateJobStatusAsync(id, request.Status, ct);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpGet("{id:guid}/applications")]
    [ProducesResponseType(typeof(PaginatedResponse<ApplicationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PaginatedResponse<ApplicationDto>>> Applications(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromServices] IApplicationService applications = null!,
        CancellationToken ct = default)
    {
        try
        {
            var result = await applications.GetApplicationsForJobAsync(
                id,
                new ApplicationListQuery(page, pageSize),
                ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
    }
}

[ApiController]
[Route("api/admin/applications")]
[Authorize(Roles = Roles.Admin)]
[Produces("application/json")]
public class AdminApplicationsController(IApplicationService applications) : ControllerBase
{
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> UpdateStatus(
        Guid id,
        [FromBody] UpdateApplicationStatusRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var application = await applications.UpdateApplicationStatusAsync(id, request.Status, ct);
        return application is null ? NotFound() : Ok(application);
    }
}
