using FluentValidation;
using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>The editable fields shared by the create and update job commands.</summary>
public sealed record JobWriteModel(
    Guid CompanyId,
    string Title,
    /// <summary>Arabic translation of Title. Optional — leave blank to fall back to Title.</summary>
    string? TitleAr,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    /// <summary>Arabic translation of Description. Optional — leave blank to fall back to Description.</summary>
    string? DescriptionAr,
    List<string> Requirements,
    /// <summary>Arabic translation of Requirements. Optional — leave empty to fall back to Requirements.</summary>
    List<string> RequirementsAr,
    List<string> Responsibilities,
    /// <summary>Arabic translation of Responsibilities. Optional — leave empty to fall back to Responsibilities.</summary>
    List<string> ResponsibilitiesAr,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate);

public sealed class JobWriteModelValidator : AbstractValidator<JobWriteModel>
{
    public JobWriteModelValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.TitleAr).MaximumLength(256);
        RuleFor(x => x.Department).NotEmpty().MinimumLength(2).MaximumLength(128);
        RuleFor(x => x.LocationType).IsInEnum();
        RuleFor(x => x.LocationText).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.JobType).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.DescriptionAr).MinimumLength(10).When(x => !string.IsNullOrEmpty(x.DescriptionAr));
        RuleFor(x => x.SalaryRange).MaximumLength(128);
        RuleFor(x => x.Status).IsInEnum();
    }
}
