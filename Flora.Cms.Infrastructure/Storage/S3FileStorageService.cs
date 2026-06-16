using Flora.Cms.Application.Abstractions.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;

namespace Flora.Cms.Infrastructure.Storage;

/// <summary>
/// Placeholder for AWS S3 integration. Wire credentials from configuration in production.
/// </summary>
public sealed class S3FileStorageService(IConfiguration config, ILogger<S3FileStorageService> logger)
    : IFileStorage
{
    private const long MaxBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
    };

    public async Task<string> UploadResumeAsync(ResumeUpload upload, CancellationToken ct = default)
    {
        if (upload.Length == 0)
            throw new ValidationException("Resume file is required.");

        if (upload.Length > MaxBytes)
            throw new ValidationException("Resume file must be 5 MB or smaller.");

        if (!AllowedContentTypes.Contains(upload.ContentType))
            throw new ValidationException("Invalid resume file type. Upload a PDF or DOCX.");

        var bucket = config["Storage:S3:Bucket"] ?? "flora-resumes";
        var key = $"applications/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{Path.GetExtension(upload.FileName)}";

        logger.LogWarning(
            "S3 upload placeholder invoked for {Key} in bucket {Bucket}. Configure AWS SDK to enable.",
            key,
            bucket);

        await using var _ = upload.Content;
        return $"https://{bucket}.s3.amazonaws.com/{key}";
    }
}
