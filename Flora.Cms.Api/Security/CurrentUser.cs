using System.Security.Claims;
using Flora.Cms.Application.Abstractions.Security;

namespace Flora.Cms.Api.Security;

/// <summary>
/// Resolves the caller from the current HTTP request's authenticated principal,
/// mirroring the previous controller logic: prefer the friendly name, then fall back
/// to the subject claim.
/// </summary>
public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string? UserName
    {
        get
        {
            var user = accessor.HttpContext?.User;
            return user?.Identity?.Name ?? user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
