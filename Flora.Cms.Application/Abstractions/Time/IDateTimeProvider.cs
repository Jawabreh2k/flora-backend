namespace Flora.Cms.Application.Abstractions.Time;

/// <summary>
/// Abstracts the system clock so handlers stay deterministic and testable instead of
/// calling <see cref="DateTimeOffset.UtcNow"/> directly.
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}
