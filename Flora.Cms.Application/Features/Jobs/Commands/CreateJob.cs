using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Mapping;
using Flora.Cms.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;

namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>Creates a new job posting under an existing company.</summary>
public sealed record CreateJobCommand(JobWriteModel Data) : ICommand<JobDetailDto>;

public sealed class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator() => RuleFor(x => x.Data).SetValidator(new JobWriteModelValidator());
}

public sealed class CreateJobCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<CreateJobCommand, JobDetailDto>
{
    public async Task<JobDetailDto> Handle(CreateJobCommand command, CancellationToken ct)
    {
        var data = command.Data;

        var companyExists = await db.Companies.AnyAsync(c => c.Id == data.CompanyId, ct);
        if (!companyExists)
            throw new ValidationException("Company not found.");

        var now = clock.UtcNow;
        var job = new JobPosting
        {
            CompanyId = data.CompanyId,
            Title = data.Title.Trim(),
            Department = data.Department.Trim(),
            LocationType = data.LocationType,
            LocationText = data.LocationText.Trim(),
            JobType = data.JobType,
            Description = data.Description.Trim(),
            Requirements = JobInputSanitizer.Clean(data.Requirements),
            Responsibilities = JobInputSanitizer.Clean(data.Responsibilities),
            SalaryRange = data.SalaryRange?.Trim(),
            Status = data.Status,
            ClosingDate = data.ClosingDate,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.JobPostings.Add(job);
        await db.SaveChangesAsync(ct);

        // Reload with its company so the detail projection is complete.
        var created = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstAsync(j => j.Id == job.Id, ct);

        return JobMappings.ToDetail(created);
    }
}
