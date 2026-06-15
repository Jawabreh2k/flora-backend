using System.ComponentModel.DataAnnotations;
using Flora.Cms.Api.Domain;

namespace Flora.Cms.Api.Dtos;

public record CompanySummaryDto(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Website);

public record CompanyDto(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Website,
    string? Description,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public record JobListItemDto(
    Guid Id,
    string Title,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string? SalaryRange,
    DateTimeOffset? ClosingDate,
    DateTimeOffset CreatedAt,
    CompanySummaryDto Company);

public record JobDetailDto(
    Guid Id,
    string Title,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<string> Responsibilities,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    CompanyDto Company);

public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasNextPage);

public record JobListQuery(
    string? Search = null,
    string? Department = null,
    LocationType? LocationType = null,
    JobType? JobType = null,
    int Page = 1,
    int PageSize = 12);

public class UpsertJobRequest
{
    [Required]
    public Guid CompanyId { get; set; }

    [Required, MinLength(2), MaxLength(256)]
    public string Title { get; set; } = string.Empty;

    [Required, MinLength(2), MaxLength(128)]
    public string Department { get; set; } = string.Empty;

    [Required]
    public LocationType LocationType { get; set; }

    [Required, MinLength(2), MaxLength(256)]
    public string LocationText { get; set; } = string.Empty;

    [Required]
    public JobType JobType { get; set; }

    [Required, MinLength(10)]
    public string Description { get; set; } = string.Empty;

    public List<string> Requirements { get; set; } = [];

    public List<string> Responsibilities { get; set; } = [];

    [MaxLength(128)]
    public string? SalaryRange { get; set; }

    [Required]
    public JobStatus Status { get; set; } = JobStatus.DRAFT;

    public DateTimeOffset? ClosingDate { get; set; }
}

public class UpdateJobStatusRequest
{
    [Required]
    public JobStatus Status { get; set; }
}

public class SubmitApplicationRequest
{
    [Required, MinLength(2), MaxLength(256)]
    public string CandidateName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(256)]
    public string CandidateEmail { get; set; } = string.Empty;

    [MaxLength(64)]
    public string? CandidatePhone { get; set; }

    [MaxLength(8000)]
    public string? CoverLetter { get; set; }
}

public record ApplicationDto(
    Guid Id,
    Guid JobPostingId,
    string CandidateName,
    string CandidateEmail,
    string? CandidatePhone,
    string ResumeUrl,
    string? CoverLetter,
    ApplicationStatus Status,
    DateTimeOffset AppliedAt);

public class UpdateApplicationStatusRequest
{
    [Required]
    public ApplicationStatus Status { get; set; }
}

public record ApplicationListQuery(int Page = 1, int PageSize = 20);

public record AdminJobListItemDto(
    Guid Id,
    string Title,
    string Department,
    JobStatus Status,
    LocationType LocationType,
    JobType JobType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int ApplicationCount,
    CompanySummaryDto Company);
