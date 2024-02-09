using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Schema;

namespace Backend.Pages;

public record Floor(int X, int Z, int XLength, int ZLength, string Color);

public class ContainersModel : PageModel
{
    public static readonly List<int> Blocks = [1, 2, 3, 4];

    public static readonly Dictionary<int, int> BlockOffsetX = new()
    {
        { 1, 2 },
        { 2, 9 },
        { 3, 2 },
        { 4, 9 },
    };

    public static readonly Dictionary<int, int> BlockOffsetZ = new()
    {
        { 1, 2 },
        { 2, 2 },
        { 3, 14 },
        { 4, 14 },
    };

    public readonly List<Floor> Floors = [
        // paths
        new(0, 0, 16, 2, "#919191"),
        new(0, 12, 16, 2, "#919191"),
        new(0, 24, 16, 2, "#919191"),
        new(0, 0, 2, 26, "#919191"),
        new(7, 0, 2, 26, "#919191"),
        new(14, 0, 2, 26, "#919191"),
        //zones
        .. (Blocks.Select(_ => new Floor(BlockOffsetX[_], BlockOffsetZ[_], 5, 10, "#000000")))
    ];

    public static Dictionary<string, string> ContainerColor = new();

    private const int CenterOffsetX = 8;
    private const int CenterOFfsetZ = 13;

    public const int FloorRotationX = -90;
    public const int FloorRotationY = 0;
    public const int FloorRotationZ = 0;

    private readonly ApplicationDbContext _context;

    public ContainersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {

    }

    public Task<List<ContainerSchema>> Containers => _context.GetContainersAsync().ContinueWith(_ => _.Result.ToList());

    public static string Rotation(double x, double y, double z) => $"{x} {y} {z}";

    public static string Position(double x, double y, double z) => $"{x - CenterOffsetX} {y} {z - CenterOFfsetZ}";
}

public static class CoordinateHelper
{
    private static readonly Random random = new();

    public static string GetPosition(this Floor floor)
    {
        return ContainersModel.Position(
            floor.X + (floor.XLength / (double)2),
            0,
            floor.Z + (floor.ZLength / (double)2));
    }

    public static string GetRotation(this Floor floor)
    {
        return ContainersModel.Rotation(ContainersModel.FloorRotationX, ContainersModel.FloorRotationY, ContainersModel.FloorRotationZ);
    }

    public static string GetPosition(this ContainerSchema container)
    {
        return ContainersModel.Position(
            0.5 + ContainersModel.BlockOffsetX[container.BlockId] + (container.BayNum - 1),
            0.5 + (container.TierNum - 1),
            1 + ContainersModel.BlockOffsetZ[container.BlockId] + ((container.StackNum - 1) * 2)
            );
    }

    public static string GetColor(this ContainerSchema container)
    {
        if (!ContainersModel.ContainerColor.TryGetValue(container.Id, out string color))
        {
            color = $"#{random.Next(256):X2}{random.Next(256):X2}{random.Next(256):X2}";
            ContainersModel.ContainerColor[container.Id] = color;
        }
        return color;
    }
}

