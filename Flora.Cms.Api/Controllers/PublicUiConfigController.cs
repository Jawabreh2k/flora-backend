using Flora.Cms.Api.Dtos;
using Flora.Cms.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Flora.Cms.Api.Controllers;

/// <summary>
/// Anonymous, read-only endpoint consumed by the public Flora Group site. The site
/// maps the returned theme tokens onto CSS variables at request time, so a config
/// change goes live without any rebuild.
/// </summary>
[ApiController]
[Route("api/ui/config")]
[AllowAnonymous]
[Produces("application/json")]
public class PublicUiConfigController(IUiConfigService configService) : ControllerBase
{
    /// <summary>Returns the live UI configuration. Supports ETag / If-None-Match caching.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(UiConfigResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var config = await configService.GetAsync(ct);

        // Revision-based ETag lets the public site (and CDNs) cache aggressively while
        // still picking up admin changes immediately.
        var etag = new EntityTagHeaderValue($"\"ui-config-v{config.Revision}\"");

        var ifNoneMatch = Request.GetTypedHeaders().IfNoneMatch;
        if (ifNoneMatch.Any(t => t.Tag == etag.Tag))
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }

        var headers = Response.GetTypedHeaders();
        headers.ETag = etag;
        headers.CacheControl = new CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromSeconds(60),
        };

        return Ok(config);
    }
}
