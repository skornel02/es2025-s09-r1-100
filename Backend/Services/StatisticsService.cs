using Backend.Options;
using Microsoft.Extensions.Options;
using Shared.Schema;
using Shared.Statistics;

namespace Backend.Services;

public class StatisticsService
{
    private readonly IOptionsMonitor<YardOptions> _yardOptions;

    public StatisticsService(IOptionsMonitor<YardOptions> yardOptions)
    {
        _yardOptions = yardOptions;
    }

    public List<BlockStatistics> GetStatistics(List<ContainerSchema> containers)
    {
        var yardOptions = _yardOptions.CurrentValue;

        var blockTotalCapacity = yardOptions.BaysPerBlock * yardOptions.StacksPerBlock * yardOptions.TiersPerBlock;
        var now = DateTime.Now;

        var statistics= containers.GroupBy(_ => _.BlockId).Select(_ =>
        {
            var usedCapacity = _.Count();
            var averageAgeInDays = _.Select(_ => now.Subtract(_.ArrivedAt).TotalDays).Average();
            var oldest = _.MinBy(_ => _.ArrivedAt);
            var youngest = _.MaxBy(_ => _.ArrivedAt);

            var baysUsed = _.Select(_ => _.BayNum).Distinct().Count();
            var stacksUsed = _.Select(_ => _.StackNum).Distinct().Count();

            return new BlockStatistics
            {
                BlockId = _.Key,
                Capacity = Math.Round(usedCapacity / (double)blockTotalCapacity, 2),
                AverageAge = Math.Round(averageAgeInDays, 2),
                OldestContainerId = oldest?.Id ?? "no containers",
                NewestContainerId = youngest?.Id ?? "no containers",
                EmptyPositions = blockTotalCapacity - usedCapacity,
                EmptyBays = yardOptions.BaysPerBlock - baysUsed,
                EmptyStacks = yardOptions.StacksPerBlock - stacksUsed,
            };
        }).ToList();

        return statistics;
    }
}
