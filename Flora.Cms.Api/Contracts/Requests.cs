using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Api.Contracts;

/// <summary>
/// HTTP request bodies. These are intentionally free of validation attributes —
/// validation is enforced uniformly by FluentValidation in the CQRS pipeline — and are
/// mapped to commands in the controllers, keeping the message contracts out of HTTP.
/// </summary>
public sealed record LoginRequest(string Username = "", string Password = "");

public sealed record UpsertJobRequest(
    Guid CompanyId,
    string Title,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    List<string>? Requirements,
    List<string>? Responsibilities,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate);

public sealed record UpdateJobStatusRequest(JobStatus Status);

public sealed record UpdateApplicationStatusRequest(ApplicationStatus Status);

/// <summary>Multipart form fields for a candidate application (the resume arrives separately).</summary>
public sealed class SubmitApplicationRequest
{
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string? CandidatePhone { get; set; }
    public string? CoverLetter { get; set; }
}
