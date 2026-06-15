using System.Text.Json;

namespace Flora.Cms.Api.Services;

/// <summary>
/// Single source of truth for how the UI payload is (de)serialized to/from the
/// nvarchar(max) column. camelCase so the persisted document matches what the
/// Next.js clients consume directly.
/// </summary>
public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Payload = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
    };
}
