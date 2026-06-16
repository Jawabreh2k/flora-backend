namespace Flora.Cms.Application.Common.Models;

/// <summary>A single page of results plus the metadata clients need to page through them.</summary>
public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    bool HasNextPage);
