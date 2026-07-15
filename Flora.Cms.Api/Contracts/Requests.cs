using Flora.Cms.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Flora.Cms.Api.Contracts;

/// <summary>
/// HTTP request bodies. These are intentionally free of validation attributes —
/// validation is enforced uniformly by FluentValidation in the CQRS pipeline — and are
/// mapped to commands in the controllers, keeping the message contracts out of HTTP.
/// </summary>
public sealed record LoginRequest(string Username = "", string Password = "");

public sealed record UpsertJobRequest(
    Guid CompanyId,
    string Title,
    string Department,
    LocationType LocationType,
    string LocationText,
    JobType JobType,
    string Description,
    List<string>? Requirements,
    List<string>? Responsibilities,
    string? SalaryRange,
    JobStatus Status,
    DateTimeOffset? ClosingDate);

public sealed record UpdateJobStatusRequest(JobStatus Status);

public sealed record UpdateApplicationStatusRequest(ApplicationStatus Status);

public sealed record UpdateContactInquiryStatusRequest(ContactInquiryStatus Status);

/// <summary>Admin email delivery settings. Secret fields are leave-blank-to-keep.</summary>
public sealed record UpdateEmailSettingsRequest(
    EmailProvider Provider,
    string FromAddress,
    string? SmtpHost,
    int SmtpPort,
    bool SmtpUseSsl,
    string? SmtpUser,
    string? SmtpPassword,
    string? ResendApiKey);

public sealed record SendTestEmailRequest(string To = "");

/// <summary>Public newsletter signup from the site footer.</summary>
public sealed record SubscribeNewsletterRequest(string Email = "", string? Locale = null);

/// <summary>
/// Multipart form fields for a candidate application, including the resume file.
/// Kept as a single [FromForm] type — mixing a complex [FromForm] parameter with a
/// separate loose IFormFile parameter on the same action breaks Swashbuckle's
/// OpenAPI schema generation (throws when building the multipart/form-data body).
/// </summary>
public sealed class SubmitApplicationRequest
{
    public string CandidateName { get; set; } = string.Empty;
    public string CandidateEmail { get; set; } = string.Empty;
    public string? CandidatePhone { get; set; }
    public string? CoverLetter { get; set; }
    public IFormFile? Resume { get; set; }
}

/// <summary>
/// Multipart form fields for an admin image upload. Kept as a single [FromForm]
/// type for the same reason as SubmitApplicationRequest — see its doc comment.
/// </summary>
public sealed class UploadRequest
{
    public IFormFile? File { get; set; }
    public string? Subsidiary { get; set; }
    public string? Field { get; set; }
}
