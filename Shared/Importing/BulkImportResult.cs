namespace Shared.Importing;

public class BulkImportResult
{
    public int Success { get; set; }
    public List<string> IncorrectPositions { get; set; } = [];
}
