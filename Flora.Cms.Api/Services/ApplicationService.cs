using Flora.Cms.Api.Data;
using Flora.Cms.Api.Domain;
using Flora.Cms.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Api.Services;

public interface IApplicationService
{
    Task<ApplicationDto> SubmitApplicationAsync(
        Guid jobPostingId,
        SubmitApplicationRequest request,
        IFormFile resume,
        CancellationToken ct);

    Task<PaginatedResponse<ApplicationDto>> GetApplicationsForJobAsync(
        Guid jobPostingId,
        ApplicationListQuery query,
        CancellationToken ct);

    Task<ApplicationDto?> UpdateApplicationStatusAsync(
        Guid applicationId,
        ApplicationStatus status,
        CancellationToken ct);
}

public sealed class ApplicationService(FloraCmsDbContext db, IFileStorageService fileStorage) : IApplicationService
{
    public async Task<ApplicationDto> SubmitApplicationAsync(
        Guid jobPostingId,
        SubmitApplicationRequest request,
        IFormFile resume,
        CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobPostingId && j.Status == JobStatus.PUBLISHED, ct);

        if (job is null)
            throw new InvalidOperationException("Job posting not found or not accepting applications.");

        if (job.ClosingDate.HasValue && job.ClosingDate.Value < DateTimeOffset.UtcNow)
            throw new InvalidOperationException("This job posting is no longer accepting applications.");

        var resumeUrl = await fileStorage.UploadResumeAsync(resume, ct);

        var application = new Application
        {
            JobPostingId = jobPostingId,
            CandidateName = request.CandidateName.Trim(),
            CandidateEmail = request.CandidateEmail.Trim(),
            CandidatePhone = request.CandidatePhone?.Trim(),
            ResumeUrl = resumeUrl,
            CoverLetter = request.CoverLetter?.Trim(),
            Status = ApplicationStatus.APPLIED,
            AppliedAt = DateTimeOffset.UtcNow,
        };

        db.Applications.Add(application);
        await db.SaveChangesAsync(ct);

        return Map(application);
    }

    public async Task<PaginatedResponse<ApplicationDto>> GetApplicationsForJobAsync(
        Guid jobPostingId,
        ApplicationListQuery query,
        CancellationToken ct)
    {
        var jobExists = await db.JobPostings.AnyAsync(j => j.Id == jobPostingId, ct);
        if (!jobExists)
            throw new InvalidOperationException("Job posting not found.");

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Applications
            .AsNoTracking()
            .Where(a => a.JobPostingId == jobPostingId);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(a => a.AppliedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(a => new ApplicationDto(
                a.Id,
                a.JobPostingId,
                a.CandidateName,
                a.CandidateEmail,
                a.CandidatePhone,
                a.ResumeUrl,
                a.CoverLetter,
                a.Status,
                a.AppliedAt))
            .ToListAsync(ct);

        return new PaginatedResponse<ApplicationDto>(items, total, page, pageSize, page * pageSize < total);
    }

    public async Task<ApplicationDto?> UpdateApplicationStatusAsync(
        Guid applicationId,
        ApplicationStatus status,
        CancellationToken ct)
    {
        var application = await db.Applications.FirstOrDefaultAsync(a => a.Id == applicationId, ct);
        if (application is null) return null;

        application.Status = status;
        await db.SaveChangesAsync(ct);
        return Map(application);
    }

    private static ApplicationDto Map(Application a) => new(
        a.Id,
        a.JobPostingId,
        a.CandidateName,
        a.CandidateEmail,
        a.CandidatePhone,
        a.ResumeUrl,
        a.CoverLetter,
        a.Status,
        a.AppliedAt);
}
