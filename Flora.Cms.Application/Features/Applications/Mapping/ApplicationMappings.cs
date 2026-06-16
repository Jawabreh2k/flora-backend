using Flora.Cms.Application.Features.Applications.Contracts;
using ApplicationEntity = Flora.Cms.Domain.Entities.Application;

namespace Flora.Cms.Application.Features.Applications.Mapping;

internal static class ApplicationMappings
{
    public static ApplicationDto ToDto(ApplicationEntity a) => new(
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
