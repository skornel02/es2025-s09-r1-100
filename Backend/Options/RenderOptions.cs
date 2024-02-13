namespace Backend.Options;

public class RenderOptions
{
    public const string SectionName = "Render";


    public int CenterOffsetX { get; set; } = 8;
    public int CenterOffsetZ { get; set; } = 13;

    public int WalkWayWidth { get; set; } = 2;
    public int ContainerWidthX { get; set; } = 1;
    public int ContainerWidthY { get; set; } = 1;
    public int ContainerWidthZ { get; set; } = 2;

}
