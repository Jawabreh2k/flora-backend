namespace Flora.Cms.Api.Services;

/// <summary>
/// Uploads candidate resumes to cloud storage. The local implementation writes to
/// <c>wwwroot/uploads/resumes</c> and returns a public URL; swap for AWS S3 in production.
/// </summary>
public interface IFileStorageService
{
    Task<string> UploadResumeAsync(IFormFile file, CancellationToken ct = default);
}

public sealed class LocalFileStorageService(IWebHostEnvironment env, ILogger<LocalFileStorageService> logger)
    : IFileStorageService
{
    private const long MaxBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    };

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf",
        ".docx",
    };

    public async Task<string> UploadResumeAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Resume file is required.");

        if (file.Length > MaxBytes)
            throw new InvalidOperationException("Resume file must be 5 MB or smaller.");

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new InvalidOperationException("Only PDF and DOCX files are allowed.");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Invalid resume file type. Upload a PDF or DOCX.");

        var uploadsDir = Path.Combine(env.WebRootPath, "uploads", "resumes");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = File.Create(fullPath);
        await file.CopyToAsync(stream, ct);

        logger.LogInformation("Stored resume at {Path} ({Bytes} bytes)", fullPath, file.Length);

        return $"/uploads/resumes/{fileName}";
    }
}

/// <summary>
/// Placeholder for AWS S3 integration. Wire credentials from configuration in production.
/// </summary>
public sealed class S3FileStorageService(IConfiguration config, ILogger<S3FileStorageService> logger)
    : IFileStorageService
{
    private const long MaxBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    };

    public async Task<string> UploadResumeAsync(IFormFile file, CancellationToken ct = default)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Resume file is required.");

        if (file.Length > MaxBytes)
            throw new InvalidOperationException("Resume file must be 5 MB or smaller.");

        if (!AllowedContentTypes.Contains(file.ContentType))
            throw new InvalidOperationException("Invalid resume file type. Upload a PDF or DOCX.");

        var bucket = config["Storage:S3:Bucket"] ?? "flora-resumes";
        var key = $"applications/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";

        logger.LogWarning(
            "S3 upload placeholder invoked for {Key} in bucket {Bucket}. Configure AWS SDK to enable.",
            key,
            bucket);

        await using var _ = file.OpenReadStream();
        return $"https://{bucket}.s3.amazonaws.com/{key}";
    }
}
