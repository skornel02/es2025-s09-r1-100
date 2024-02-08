using Shared.Schema;

namespace Backend.Extensions;

public static class ContainerImportExtension
{
    public static List<ContainerSchema> ParseContainers(this List<List<string>> csv)
    {
        var containers = new List<ContainerSchema>();
        var dataRows = csv.Skip(1);

        foreach (var row in dataRows)
        {
            containers.Add(new()
            {
                Id = row[0],
                BlockId = int.Parse(row[1]),
                BayNum = int.Parse(row[2]),
                StackNum = int.Parse(row[3]),
                TierNum = int.Parse(row[4]),
                ArrivedAt = DateTime.UnixEpoch.AddMilliseconds(long.Parse(row[5]))
            });
        }

        return containers;
    }
}
