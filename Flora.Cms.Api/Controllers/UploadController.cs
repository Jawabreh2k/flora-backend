using System.Text.RegularExpressions;
using Flora.Cms.Api.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flora.Cms.Api.Controllers;

/// <summary>
/// Handles file uploads (images) for the Admin Portal.
/// Stores files in a local uploads directory, which should be mapped to CDN in production.
/// </summary>
[ApiController]
[Route("api/upload")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public partial class UploadController : ControllerBase
{
    private readonly string _uploadDir;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    // Only [a-z0-9-] survives; everything else (including '.', '/', '\') is stripped,
    // so a hostile `subsidiary` value can never escape the uploads directory.
    [GeneratedRegex("[^a-zA-Z0-9-]")]
    private static partial Regex UnsafeSlugChars();

    public UploadController(IWebHostEnvironment env)
    {
        // WebRootPath is null until wwwroot exists; fall back to a content-root path.
        var webRoot = env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot");
        _uploadDir = Path.Combine(webRoot, "uploads");

        // Ensure upload directory exists
        if (!Directory.Exists(_uploadDir))
        {
            Directory.CreateDirectory(_uploadDir);
        }
    }

    /// <summary>
    /// Uploads an image file for a subsidiary.
    /// POST /api/upload
    /// Form data:
    ///   - file: IFormFile (required)
    ///   - subsidiary: string (required, subsidiary slug)
    ///   - field: string (required, "image" or "logo")
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
    public async Task<ActionResult<UploadResponse>> Upload([FromForm] UploadRequest request)
    {
        var file = request.File;
        var subsidiary = request.Subsidiary;
        var field = request.Field;

        // Validate inputs
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file provided." });

        if (string.IsNullOrWhiteSpace(subsidiary))
            return BadRequest(new { error = "Subsidiary slug required." });

        if (field != "image" && field != "logo")
            return BadRequest(new { error = "Field must be 'image' or 'logo'." });

        if (file.Length > MaxFileSize)
            return StatusCode(StatusCodes.Status413PayloadTooLarge,
                new { error = $"File too large. Maximum size: 10MB." });

        // Validate file type by extension...
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return BadRequest(new { error = "Invalid file type. Allowed: jpg, jpeg, png, webp, gif." });

        // ...and by content: confirm the bytes actually match a known image signature so
        // a renamed script/HTML payload can't be stored and later served as an image.
        if (!await IsRecognisedImageAsync(file))
            return BadRequest(new { error = "File content does not match a supported image format." });

        try
        {
            // Sanitize the caller-supplied slug to a safe token and add a random
            // component, so the filename can never contain path separators or '..'
            // and two uploads never collide / overwrite each other.
            var safeSlug = UnsafeSlugChars().Replace(subsidiary, "");
            if (string.IsNullOrEmpty(safeSlug))
                return BadRequest(new { error = "Subsidiary slug is invalid." });

            var filename = $"{safeSlug}-{field}-{Guid.NewGuid():N}{ext}";
            var filepath = Path.Combine(_uploadDir, filename);

            // Save file
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return full public URL (admin portal and website need to access this from the backend)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var publicUrl = $"{baseUrl}/uploads/{filename}";

            return Ok(new UploadResponse
            {
                Url = publicUrl,
                Filename = filename,
            });
        }
        catch
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Upload failed. Please try again." });
        }
    }

    /// <summary>
    /// Reads the leading bytes and checks them against the magic numbers for the
    /// formats we accept (JPEG, PNG, GIF, WEBP). Extension/content-type alone are
    /// trivially spoofable, so this is the actual gate on what gets persisted.
    /// </summary>
    private static async Task<bool> IsRecognisedImageAsync(IFormFile file)
    {
        const int headerLength = 12;
        var header = new byte[headerLength];

        await using var stream = file.OpenReadStream();
        // ReadAtLeastAsync keeps reading until the buffer is full or EOF, so a short
        // first read can't falsely reject a legitimate image.
        var read = await stream.ReadAtLeastAsync(header, headerLength, throwOnEndOfStream: false);
        if (read < headerLength)
            return false;

        // JPEG: FF D8 FF
        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return true;

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
            header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A)
            return true;

        // GIF: "GIF8"
        if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38)
            return true;

        // WEBP: "RIFF" .... "WEBP"
        if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
            header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
            return true;

        return false;
    }
}

/// <summary>Response from upload endpoint.</summary>
public class UploadResponse
{
    public string Url { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;
}
