namespace Flora.Cms.Api.Domain;

public enum LocationType
{
    REMOTE,
    HYBRID,
    ONSITE,
}

public enum JobType
{
    FULL_TIME,
    PART_TIME,
    CONTRACT,
    INTERN,
}

public enum JobStatus
{
    DRAFT,
    PUBLISHED,
    ARCHIVED,
}

public enum ApplicationStatus
{
    APPLIED,
    SCREENING,
    INTERVIEWING,
    OFFERED,
    REJECTED,
}
