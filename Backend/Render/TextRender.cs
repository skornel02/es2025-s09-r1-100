namespace Backend.Render;

public record TextRender
{
    public required string Text { get; set; }
    public required string Position { get; init; }
    public required string Rotation { get; init; }
}
