using Flora.Cms.Api.Domain;

namespace Flora.Cms.Api.Services;

public interface ITokenService
{
    (string token, DateTimeOffset expiresAtUtc) CreateAccessToken(AdminUser user);
}
