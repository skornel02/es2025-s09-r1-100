namespace Backend.Render;

public record TableRender
{
    public required string Id { get; init; }

    public required string LevelName { get; init; }

    public required int Level { get; init; }
    public required int RowCount { get; init; }
    public required int ColumnCount { get; init; }

    public required int[] PathRowIndexes { get; init; }
    public required int[] PathColumnIndexes { get; init; }
}
