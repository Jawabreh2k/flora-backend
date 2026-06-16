using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Flora.Cms.Infrastructure.Persistence.Converters;

/// <summary>Persists <see cref="List{T}"/> of strings as JSON in SQL Server.</summary>
public static class JsonStringListConverter
{
    private static readonly JsonSerializerOptions Options = new();

    public static ValueConverter<List<string>, string> Instance { get; } = new(
        v => JsonSerializer.Serialize(v, Options),
        v => JsonSerializer.Deserialize<List<string>>(v, Options) ?? new List<string>());

    public static ValueComparer<List<string>> Comparer { get; } = new(
        (a, b) => (a ?? new List<string>()).SequenceEqual(b ?? new List<string>()),
        v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
        v => v.ToList());
}
