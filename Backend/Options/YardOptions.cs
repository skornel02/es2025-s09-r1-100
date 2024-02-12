namespace Backend.Options;

public class YardOptions
{
    public const string SectionName = "Yard";

    public int BlockAmount { get; set; } = 4;
    public int BaysPerBlock { get; set; } = 5;
    public int StacksPerBlock { get; set; } = 5;
    public int TiersPerBlock { get; set; } = 5;
}
