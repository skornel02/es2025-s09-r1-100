namespace Shared.Schema;

public class ContainerSchema
{
    public required string Id { get; set; }
    public required int BlockId { get; set; }
    public required int BayNum { get; set; }
    public required int StackNum { get; set; }
    public required int TierNum { get; set; }
    public required DateTime ArrivedAt { get; set; }
}
