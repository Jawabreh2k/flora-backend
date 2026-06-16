using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Mapping;
using Microsoft.EntityFrameworkCore;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>Replaces all editable fields of an existing job posting.</summary>
public sealed record UpdateJobCommand(Guid Id, JobWriteModel Data) : ICommand<JobDetailDto>;

public sealed class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Data).SetValidator(new JobWriteModelValidator());
    }
}

public sealed class UpdateJobCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<UpdateJobCommand, JobDetailDto>
{
    public async Task<JobDetailDto> Handle(UpdateJobCommand command, CancellationToken ct)
    {
        var data = command.Data;

        var job = await db.JobPostings
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == command.Id, ct);

        if (job is null)
            throw new NotFoundException("Job posting not found.");

        var companyExists = await db.Companies.AnyAsync(c => c.Id == data.CompanyId, ct);
        if (!companyExists)
            throw new ValidationException("Company not found.");

        job.CompanyId = data.CompanyId;
        job.Title = data.Title.Trim();
        job.Department = data.Department.Trim();
        job.LocationType = data.LocationType;
        job.LocationText = data.LocationText.Trim();
        job.JobType = data.JobType;
        job.Description = data.Description.Trim();
        job.Requirements = JobInputSanitizer.Clean(data.Requirements);
        job.Responsibilities = JobInputSanitizer.Clean(data.Responsibilities);
        job.SalaryRange = data.SalaryRange?.Trim();
        job.Status = data.Status;
        job.ClosingDate = data.ClosingDate;
        job.UpdatedAt = clock.UtcNow;

        await db.SaveChangesAsync(ct);

        // The Company nav can change; reload the row with its company for the projection.
        var updated = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstAsync(j => j.Id == job.Id, ct);

        return JobMappings.ToDetail(updated);
    }
}
