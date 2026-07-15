using Flora.Cms.Api.Contracts;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Storage;
using Flora.Cms.Application.Common.Models;
using Flora.Cms.Application.Features.Applications.Commands;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Queries;
using Flora.Cms.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicJobsController(ISender sender) : ControllerBase
{
    private const long MaxResumeBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedResumeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".docx",
    };

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
        => Ok(await sender.Send(
            new GetPublishedJobsQuery(search, department, locationType, jobType, page, pageSize), ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(JobDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JobDetailDto>> Get(Guid id, CancellationToken ct)
    {
        var job = await sender.Send(new GetPublishedJobQuery(id), ct);
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
        CancellationToken ct)
    {
        var resume = request.Resume;
        var resumeError = ValidateResume(resume);
        if (resumeError is not null)
            return BadRequest(new ProblemDetails { Title = resumeError });

        await using var stream = resume!.OpenReadStream();
        var command = new SubmitApplicationCommand(
            id,
            request.CandidateName,
            request.CandidateEmail,
            request.CandidatePhone,
            request.CoverLetter,
            new ResumeUpload(stream, resume.FileName, resume.ContentType, resume.Length));

        var application = await sender.Send(command, ct);
        return CreatedAtAction(nameof(Get), new { id }, application);
    }

    private static string? ValidateResume(IFormFile? resume)
    {
        if (resume is null || resume.Length == 0)
            return "Resume file is required.";

        if (resume.Length > MaxResumeBytes)
            return "Resume file must be 5 MB or smaller.";

        var extension = Path.GetExtension(resume.FileName);
        if (!AllowedResumeExtensions.Contains(extension))
            return "Only PDF and DOCX files are allowed.";

        return null;
    }
}
