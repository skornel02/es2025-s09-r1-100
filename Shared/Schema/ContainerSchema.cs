namespace Shared.Schema;

public class ContainerSchema
{
    public required string Id { get; set; }

    public required int BlockId { get; set; }

    /// <summary>
    /// X coordinate
    /// </summary>
    public required int BayNum { get; set; }

    /// <summary>
    /// Z coordinate
    /// </summary>
    public required int StackNum { get; set; }

    /// <summary>
    /// Y coordinate 
    /// </summary>
    public required int TierNum { get; set; }

    public required DateTime ArrivedAt { get; set; }
}
