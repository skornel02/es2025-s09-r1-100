namespace Shared.Utility;

public record DatabaseQueryParams(
    int PageStart,
    int PageSize,
    string? Sorting,
    string? Ordering,
    Dictionary<string, string> Search,
    string? GlobalSearch
);
