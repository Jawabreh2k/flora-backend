using Flora.Cms.Api.Domain;
using Flora.Cms.Api.Dtos;
using Flora.Cms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicJobsController(IJobService jobs, IApplicationService applications) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<JobListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponse<JobListItemDto>>> List(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] LocationType? locationType,
        [FromQuery] JobType? jobType,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        CancellationToken ct = default)
    {
        var result = await jobs.GetPublishedJobsAsync(
            new JobListQuery(search, department, locationType, jobType, page, pageSize),
            ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var job = await jobs.GetPublishedJobAsync(id, ct);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpPost("{id:guid}/apply")]
    [RequestSizeLimit(6 * 1024 * 1024)]
    [ProducesResponseType(typeof(ApplicationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Apply(
        Guid id,
        [FromForm] SubmitApplicationRequest request,
        IFormFile? resume,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        if (resume is null || resume.Length == 0)
            return BadRequest(new ProblemDetails { Title = "Resume file is required." });

        try
        {
            var application = await applications.SubmitApplicationAsync(id, request, resume, ct);
            return CreatedAtAction(nameof(Get), new { id }, application);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new ProblemDetails { Title = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message });
        }
    }
}
