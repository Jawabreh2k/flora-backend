namespace Flora.Cms.Api.Domain;

public class Application
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobPostingId { get; set; }

    public JobPosting JobPosting { get; set; } = null!;

    public required string CandidateName { get; set; }

    public required string CandidateEmail { get; set; }

    public string? CandidatePhone { get; set; }

    public required string ResumeUrl { get; set; }

    public string? CoverLetter { get; set; }

    public ApplicationStatus Status { get; set; } = ApplicationStatus.APPLIED;

    public DateTimeOffset AppliedAt { get; set; } = DateTimeOffset.UtcNow;
}
