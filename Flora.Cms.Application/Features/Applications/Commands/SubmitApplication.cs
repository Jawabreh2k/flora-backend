using FluentValidation;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Storage;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Application.Features.Applications.Mapping;
using Flora.Cms.Domain.Enums;
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;
using Microsoft.EntityFrameworkCore;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;
using NotFoundException = Flora.Cms.Application.Common.Exceptions.NotFoundException;

namespace Flora.Cms.Application.Features.Applications.Commands;

/// <summary>A candidate applies to a published job posting with a resume.</summary>
public sealed record SubmitApplicationCommand(
    Guid JobPostingId,
    string CandidateName,
    string CandidateEmail,
    string? CandidatePhone,
    string? CoverLetter,
    ResumeUpload Resume) : ICommand<ApplicationDto>;

public sealed class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    private const long MaxResumeBytes = 5 * 1024 * 1024;

    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.CandidateName).NotEmpty().MinimumLength(2).MaximumLength(256);
        RuleFor(x => x.CandidateEmail).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.CandidatePhone).MaximumLength(64);
        RuleFor(x => x.CoverLetter).MaximumLength(8000);
        RuleFor(x => x.Resume.Length)
            .GreaterThan(0)
            .WithMessage("Resume file is required.");
        RuleFor(x => x.Resume.Length)
            .LessThanOrEqualTo(MaxResumeBytes)
            .WithMessage("Resume file must be 5 MB or smaller.");
        RuleFor(x => x.Resume.FileName)
            .Must(HasAllowedResumeExtension)
            .WithMessage("Only PDF and DOCX files are allowed.");
    }

    private static bool HasAllowedResumeExtension(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        return extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase)
            || extension.Equals(".docx", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class SubmitApplicationCommandHandler(
    IAppDbContext db,
    IFileStorage fileStorage,
    IDateTimeProvider clock)
    : ICommandHandler<SubmitApplicationCommand, ApplicationDto>
{
    public async Task<ApplicationDto> Handle(SubmitApplicationCommand command, CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == command.JobPostingId && j.Status == JobStatus.PUBLISHED, ct);

        if (job is null)
            throw new NotFoundException("Job posting not found or not accepting applications.");

        if (job.ClosingDate.HasValue && job.ClosingDate.Value < clock.UtcNow)
            throw new ValidationException("This job posting is no longer accepting applications.");

        var resumeUrl = await fileStorage.UploadResumeAsync(command.Resume, ct);

        var application = new ApplicationEntity
        {
            JobPostingId = command.JobPostingId,
            CandidateName = command.CandidateName.Trim(),
            CandidateEmail = command.CandidateEmail.Trim(),
            CandidatePhone = command.CandidatePhone?.Trim(),
            ResumeUrl = resumeUrl,
            CoverLetter = command.CoverLetter?.Trim(),
            Status = ApplicationStatus.APPLIED,
            AppliedAt = clock.UtcNow,
        };

        db.Applications.Add(application);
        await db.SaveChangesAsync(ct);

        return ApplicationMappings.ToDto(application);
    }
}
