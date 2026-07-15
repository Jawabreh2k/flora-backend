using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Domain.Entities;

namespace Flora.Cms.Application.Features.Jobs.Mapping;

/// <summary>Shared entity → contract projections used across the Jobs handlers.</summary>
internal static class JobMappings
{
    public static JobDetailDto ToDetail(JobPosting j) => new(
        j.Id,
        j.Title,
        j.TitleAr,
        j.Department,
        j.LocationType,
        j.LocationText,
        j.JobType,
        j.Description,
        j.DescriptionAr,
        j.Requirements,
        j.RequirementsAr,
        j.Responsibilities,
        j.ResponsibilitiesAr,
        j.SalaryRange,
        j.Status,
        j.ClosingDate,
        j.CreatedAt,
        j.UpdatedAt,
        new CompanyDto(
            j.Company.Id,
            j.Company.Name,
            j.Company.LogoUrl,
            j.Company.Website,
            j.Company.Description,
            j.Company.CreatedAt,
            j.Company.UpdatedAt));
}
