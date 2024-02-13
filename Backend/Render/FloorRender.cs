namespace Backend.Render;

public record FloorRender
{
    public required string Position { get; init; }
    public required string Rotation { get; init; }
    public required int XLength { get; init; }
    public required int ZLength { get; init; }
    public required string Color { get; init; }
}
