using FluentValidation;
using Flora.Cms.Application.Abstractions.Careers;
using Flora.Cms.Application.Abstractions.Email;
using Flora.Cms.Application.Abstractions.Messaging;
using Flora.Cms.Application.Abstractions.Persistence;
using Flora.Cms.Application.Abstractions.Storage;
using Flora.Cms.Application.Abstractions.Time;
using Flora.Cms.Application.Features.Applications.Contracts;
using Flora.Cms.Application.Features.Applications.Mapping;
using Flora.Cms.Domain.Enums;
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;
using JobPostingEntity = Flora.Cms.Domain.Entities.JobPosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    IDateTimeProvider clock,
    ICareersRecipientResolver recipientResolver,
    IEmailSender emailSender,
    ILogger<SubmitApplicationCommandHandler> logger)
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

        await NotifyHrAsync(application, job, ct);
        await SendCandidateConfirmationAsync(application, job, ct);

        return ApplicationMappings.ToDto(application);
    }

    /// <summary>
    /// Emails HR that a new application arrived. Best-effort: the candidate's submission
    /// has already been persisted, so a delivery failure is logged but never surfaced.
    /// </summary>
    private async Task NotifyHrAsync(ApplicationEntity application, JobPostingEntity job, CancellationToken ct)
    {
        try
        {
            var recipient = await recipientResolver.ResolveAsync("en", ct);

            static string Enc(string? value) => System.Net.WebUtility.HtmlEncode(value ?? "—");

            var coverLetter = string.IsNullOrWhiteSpace(application.CoverLetter)
                ? "<p><em>No cover letter provided.</em></p>"
                : $"<p>{Enc(application.CoverLetter).Replace("\n", "<br/>")}</p>";

            var html = $"""
                <h2>New job application</h2>
                <p><strong>Role:</strong> {Enc(job.Title)} — {Enc(job.Department)}</p>
                <p><strong>Candidate:</strong> {Enc(application.CandidateName)}</p>
                <p><strong>Email:</strong> {Enc(application.CandidateEmail)}</p>
                <p><strong>Phone:</strong> {Enc(application.CandidatePhone)}</p>
                <hr />
                <p><strong>Cover letter</strong></p>
                {coverLetter}
                <p><strong>Résumé:</strong> {Enc(application.ResumeUrl)}</p>
                <p style="color:#888;font-size:12px">Review and respond in the Flora admin portal.</p>
                """;

            await emailSender.SendAsync(
                new EmailMessage(
                    recipient,
                    $"[Flora Careers] New application: {application.CandidateName} — {job.Title}",
                    html,
                    application.CandidateEmail),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send application alert for application {ApplicationId}", application.Id);
        }
    }

    /// <summary>
    /// Sends the candidate a receipt confirming their application was received. Without
    /// this, a candidate whose browser closes right after submitting has no record they
    /// ever applied. Best-effort, same as NotifyHrAsync — the application is already saved.
    /// </summary>
    private async Task SendCandidateConfirmationAsync(ApplicationEntity application, JobPostingEntity job, CancellationToken ct)
    {
        try
        {
            var hrRecipient = await recipientResolver.ResolveAsync("en", ct);

            static string Enc(string? value) => System.Net.WebUtility.HtmlEncode(value ?? "—");

            var html = $"""
                <h2>Thank you for applying, {Enc(application.CandidateName)}!</h2>
                <p>We've received your application for <strong>{Enc(job.Title)}</strong> at Flora Group.</p>
                <p>Our HR team will review your profile and reach out if there's a match.</p>
                <hr />
                <p style="color:#888;font-size:12px">This is an automated confirmation — please don't reply directly to this address.</p>
                """;

            await emailSender.SendAsync(
                new EmailMessage(
                    application.CandidateEmail,
                    $"Application received — {job.Title} at Flora Group",
                    html,
                    hrRecipient),
                ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send candidate confirmation for application {ApplicationId}", application.Id);
        }
    }
}
