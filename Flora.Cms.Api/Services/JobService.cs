using Flora.Cms.Api.Data;
using Flora.Cms.Api.Domain;
using Flora.Cms.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Flora.Cms.Api.Services;

public interface IJobService
{
    Task<PaginatedResponse<JobListItemDto>> GetPublishedJobsAsync(JobListQuery query, CancellationToken ct);
    Task<JobDetailDto?> GetPublishedJobAsync(Guid id, CancellationToken ct);
    Task<PaginatedResponse<AdminJobListItemDto>> GetAdminJobsAsync(int page, int pageSize, CancellationToken ct);
    Task<JobDetailDto?> GetAdminJobAsync(Guid id, CancellationToken ct);
    Task<JobDetailDto> CreateJobAsync(UpsertJobRequest request, CancellationToken ct);
    Task<JobDetailDto?> UpdateJobAsync(Guid id, UpsertJobRequest request, CancellationToken ct);
    Task<JobDetailDto?> UpdateJobStatusAsync(Guid id, JobStatus status, CancellationToken ct);
    Task<IReadOnlyList<CompanyDto>> GetCompaniesAsync(CancellationToken ct);
}

public sealed class JobService(FloraCmsDbContext db) : IJobService
{
    public async Task<PaginatedResponse<JobListItemDto>> GetPublishedJobsAsync(
        JobListQuery query,
        CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 50);

        var q = db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .Where(j => j.Status == JobStatus.PUBLISHED);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(j =>
                j.Title.Contains(term) ||
                j.Company.Name.Contains(term) ||
                j.Department.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(query.Department))
            q = q.Where(j => j.Department == query.Department);

        if (query.LocationType.HasValue)
            q = q.Where(j => j.LocationType == query.LocationType.Value);

        if (query.JobType.HasValue)
            q = q.Where(j => j.JobType == query.JobType.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobListItemDto(
                j.Id,
                j.Title,
                j.Department,
                j.LocationType,
                j.LocationText,
                j.JobType,
                j.SalaryRange,
                j.ClosingDate,
                j.CreatedAt,
                new CompanySummaryDto(j.Company.Id, j.Company.Name, j.Company.LogoUrl, j.Company.Website)))
            .ToListAsync(ct);

        return new PaginatedResponse<JobListItemDto>(
            items,
            total,
            page,
            pageSize,
            page * pageSize < total);
    }

    public async Task<JobDetailDto?> GetPublishedJobAsync(Guid id, CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == id && j.Status == JobStatus.PUBLISHED, ct);

        return job is null ? null : MapDetail(job);
    }

    public async Task<PaginatedResponse<AdminJobListItemDto>> GetAdminJobsAsync(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var q = db.JobPostings.AsNoTracking().Include(j => j.Company);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(j => j.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new AdminJobListItemDto(
                j.Id,
                j.Title,
                j.Department,
                j.Status,
                j.LocationType,
                j.JobType,
                j.CreatedAt,
                j.UpdatedAt,
                j.Applications.Count,
                new CompanySummaryDto(j.Company.Id, j.Company.Name, j.Company.LogoUrl, j.Company.Website)))
            .ToListAsync(ct);

        return new PaginatedResponse<AdminJobListItemDto>(items, total, page, pageSize, page * pageSize < total);
    }

    public async Task<JobDetailDto?> GetAdminJobAsync(Guid id, CancellationToken ct)
    {
        var job = await db.JobPostings
            .AsNoTracking()
            .Include(j => j.Company)
            .FirstOrDefaultAsync(j => j.Id == id, ct);

        return job is null ? null : MapDetail(job);
    }

    public async Task<JobDetailDto> CreateJobAsync(UpsertJobRequest request, CancellationToken ct)
    {
        var companyExists = await db.Companies.AnyAsync(c => c.Id == request.CompanyId, ct);
        if (!companyExists)
            throw new InvalidOperationException("Company not found.");

        var now = DateTimeOffset.UtcNow;
        var job = new JobPosting
        {
            CompanyId = request.CompanyId,
            Title = request.Title.Trim(),
            Department = request.Department.Trim(),
            LocationType = request.LocationType,
            LocationText = request.LocationText.Trim(),
            JobType = request.JobType,
            Description = request.Description.Trim(),
            Requirements = SanitizeList(request.Requirements),
            Responsibilities = SanitizeList(request.Responsibilities),
            SalaryRange = request.SalaryRange?.Trim(),
            Status = request.Status,
            ClosingDate = request.ClosingDate,
            CreatedAt = now,
            UpdatedAt = now,
        };

        db.JobPostings.Add(job);
        await db.SaveChangesAsync(ct);

        await db.Entry(job).Reference(j => j.Company).LoadAsync(ct);
        return MapDetail(job);
    }

    public async Task<JobDetailDto?> UpdateJobAsync(Guid id, UpsertJobRequest request, CancellationToken ct)
    {
        var job = await db.JobPostings.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id, ct);
        if (job is null) return null;

        var companyExists = await db.Companies.AnyAsync(c => c.Id == request.CompanyId, ct);
        if (!companyExists)
            throw new InvalidOperationException("Company not found.");

        job.CompanyId = request.CompanyId;
        job.Title = request.Title.Trim();
        job.Department = request.Department.Trim();
        job.LocationType = request.LocationType;
        job.LocationText = request.LocationText.Trim();
        job.JobType = request.JobType;
        job.Description = request.Description.Trim();
        job.Requirements = SanitizeList(request.Requirements);
        job.Responsibilities = SanitizeList(request.Responsibilities);
        job.SalaryRange = request.SalaryRange?.Trim();
        job.Status = request.Status;
        job.ClosingDate = request.ClosingDate;
        job.UpdatedAt = DateTimeOffset.UtcNow;

        await db.SaveChangesAsync(ct);
        return MapDetail(job);
    }

    public async Task<JobDetailDto?> UpdateJobStatusAsync(Guid id, JobStatus status, CancellationToken ct)
    {
        var job = await db.JobPostings.Include(j => j.Company).FirstOrDefaultAsync(j => j.Id == id, ct);
        if (job is null) return null;

        job.Status = status;
        job.UpdatedAt = DateTimeOffset.UtcNow;
        await db.SaveChangesAsync(ct);
        return MapDetail(job);
    }

    public async Task<IReadOnlyList<CompanyDto>> GetCompaniesAsync(CancellationToken ct)
        => await db.Companies
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CompanyDto(
                c.Id,
                c.Name,
                c.LogoUrl,
                c.Website,
                c.Description,
                c.CreatedAt,
                c.UpdatedAt))
            .ToListAsync(ct);

    private static List<string> SanitizeList(IEnumerable<string> items)
        => items
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    private static JobDetailDto MapDetail(JobPosting j) => new(
        j.Id,
        j.Title,
        j.Department,
        j.LocationType,
        j.LocationText,
        j.JobType,
        j.Description,
        j.Requirements,
        j.Responsibilities,
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
