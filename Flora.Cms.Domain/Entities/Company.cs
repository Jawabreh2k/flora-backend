namespace Flora.Cms.Domain.Entities;

public class Company
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public required string Name { get; set; }

    public string? LogoUrl { get; set; }

    public string? Website { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
