using Flora.Cms.Application.Abstractions.Storage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ValidationException = Flora.Cms.Application.Common.Exceptions.ValidationException;

namespace Flora.Cms.Infrastructure.Storage;

/// <summary>
/// Writes candidate resumes under <c>{ContentRoot}/wwwroot/uploads/resumes</c> and
/// returns a site-relative URL that the API serves via static files. Swap for
/// <see cref="S3FileStorageService"/> in production.
/// </summary>
public sealed class LocalFileStorageService(IHostEnvironment env, ILogger<LocalFileStorageService> logger)
    : IFileStorage
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

    public async Task<string> UploadResumeAsync(ResumeUpload upload, CancellationToken ct = default)
    {
        if (upload.Length == 0)
            throw new ValidationException("Resume file is required.");

        if (upload.Length > MaxBytes)
            throw new ValidationException("Resume file must be 5 MB or smaller.");

        var extension = Path.GetExtension(upload.FileName);
        if (!AllowedExtensions.Contains(extension))
            throw new ValidationException("Only PDF and DOCX files are allowed.");

        if (!AllowedContentTypes.Contains(upload.ContentType))
            throw new ValidationException("Invalid resume file type. Upload a PDF or DOCX.");

        var uploadsDir = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "resumes");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsDir, fileName);

        await using var stream = File.Create(fullPath);
        await upload.Content.CopyToAsync(stream, ct);

        logger.LogInformation("Stored resume at {Path} ({Bytes} bytes)", fullPath, upload.Length);

        return $"/uploads/resumes/{fileName}";
    }
}
