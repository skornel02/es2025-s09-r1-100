namespace Shared.Utility;

public record AjaxContainer<T>(
    int Draw,
    int RecordsTotal,
    int RecordsFiltered,
    List<T> Data
) where T : class;
