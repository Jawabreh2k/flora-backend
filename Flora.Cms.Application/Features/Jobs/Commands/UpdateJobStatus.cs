using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Jobs.Contracts;
using Flora.Cms.Application.Features.Jobs.Mapping;
using Flora.Cms.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Jobs.Commands;

/// <summary>Transitions a job posting to a new lifecycle status (draft/published/archived).</summary>
public sealed record UpdateJobStatusCommand(Guid Id, JobStatus Status) : ICommand<JobDetailDto>;

public sealed class UpdateJobStatusCommandValidator : AbstractValidator<UpdateJobStatusCommand>
{
    public UpdateJobStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
    }
}

public sealed class UpdateJobStatusCommandHandler(IAppDbContext db, IDateTimeProvider clock)
    : ICommandHandler<UpdateJobStatusCommand, JobDetailDto>
{
    public async Task<JobDetailDto> Handle(UpdateJobStatusCommand command, CancellationToken ct)
    {
        var job = await db.JobPostings
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == command.Id, ct);

        if (job is null)
            throw new NotFoundException("Job posting not found.");

        job.Status = command.Status;
        job.UpdatedAt = clock.UtcNow;
        await db.SaveChangesAsync(ct);

        return JobMappings.ToDetail(job);
    }
}
