using Flora.Cms.Domain.Enums;

namespace Flora.Cms.Application.Features.Applications.Contracts;

public record ApplicationDto(
    Guid Id,
    Guid JobPostingId,
    string CandidateName,
    string CandidateEmail,
    string? CandidatePhone,
    string ResumeUrl,
    string? CoverLetter,
    ApplicationStatus Status,
    DateTimeOffset AppliedAt);
