namespace Flora.Cms.Application.Abstractions.Storage;

/// <summary>
/// A resume to persist. Framework-agnostic on purpose: the API adapts its
/// <c>IFormFile</c> into this so the Application/Infrastructure layers never depend
/// on ASP.NET's HTTP types.
/// </summary>
public sealed record ResumeUpload(Stream Content, string FileName, string ContentType, long Length);

/// <summary>
/// Stores candidate resumes in durable storage and returns a publicly retrievable
/// URL. Local disk and S3 implementations live in Infrastructure.
/// </summary>
public interface IFileStorage
{
    Task<string> UploadResumeAsync(ResumeUpload upload, CancellationToken ct = default);
}
