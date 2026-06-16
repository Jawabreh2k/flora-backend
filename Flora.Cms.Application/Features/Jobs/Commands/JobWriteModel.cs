using FluentValidation;
using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>The editable fields shared by the create and update job commands.</summary>
public sealed record JobWriteModel(
    Guid CompanyId,
    string Title,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    List<string> Requirements,
    List<string> Responsibilities,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate);

public sealed class JobWriteModelValidator : AbstractValidator<JobWriteModel>
{
    public JobWriteModelValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.Department).NotEmpty().MinimumLength(2).MaximumLength(128);
        RuleFor(x => x.LocationType).IsInEnum();
        RuleFor(x => x.LocationText).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.JobType).IsInEnum();
        RuleFor(x => x.Description).NotEmpty().MinimumLength(10);
        RuleFor(x => x.SalaryRange).MaximumLength(128);
        RuleFor(x => x.Status).IsInEnum();
    }
}
