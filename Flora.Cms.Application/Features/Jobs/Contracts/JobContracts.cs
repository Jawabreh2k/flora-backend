using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Jobs.Contracts;

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
    /// <summary>Arabic translation of Title, when an editor has provided one.</summary>
    string? TitleAr,
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
    string? TitleAr,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    string? DescriptionAr,
    IReadOnlyList<string> Requirements,
    IReadOnlyList<string> RequirementsAr,
    IReadOnlyList<string> Responsibilities,
    IReadOnlyList<string> ResponsibilitiesAr,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    CompanyDto Company);

public record AdminJobListItemDto(
    Guid Id,
    string Title,
    string? TitleAr,
    string Department,
    JobStatus Status,
    LocationType LocationType,
    JobType JobType,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    int ApplicationCount,
    CompanySummaryDto Company);
