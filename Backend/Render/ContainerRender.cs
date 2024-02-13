namespace Backend.Render;

public record ContainerRender
{
    public required string Id { get; set; }

    public required string Position { get; init; }
    public required string Rotation { get; init; }
    public required int XLength { get; init; }
    public required int YLength { get; init; }
    public required int ZLength { get; init; }
    public required string Color { get; init; }
}
