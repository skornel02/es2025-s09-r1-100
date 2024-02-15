namespace Shared.Utility;

public record DatabaseResponse<T>(
    int TotalCount,
    List<T> Data
) where T: class;
