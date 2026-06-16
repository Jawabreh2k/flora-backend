using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Domain.Entities;

public class JobPosting
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid CompanyId { get; set; }

    public Company Company { get; set; } = null!;

    public required string Title { get; set; }

    public required string Department { get; set; }

    public LocationType LocationType { get; set; }

    public required string LocationText { get; set; }

    public JobType JobType { get; set; }

    public required string Description { get; set; }

    public List<string> Requirements { get; set; } = [];

    public List<string> Responsibilities { get; set; } = [];

    public string? SalaryRange { get; set; }

    public JobStatus Status { get; set; } = JobStatus.DRAFT;

    public DateTimeOffset? ClosingDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}
