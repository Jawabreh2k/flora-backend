using Flora.Cms.Application.Abstractions.Time;

namespace Flora.Cms.Infrastructure.Time;

/// <summary>Production clock backed by the operating system's UTC time.</summary>
public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
