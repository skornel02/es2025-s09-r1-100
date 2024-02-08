namespace Shared.Statistics;

public class BlockStatistics
{
    public int BlockId { get; set; }
    public double Capacity { get; set; }
    public double AverageAge { get; set; }
    public string OldestContainerId { get; set; } = default!;
    public string NewestContainerId { get; set; } = default!;
    public int EmptyPositions { get; set; }
    public int EmptyBays { get; set; }
    public int EmptyStacks { get; set; }
}
