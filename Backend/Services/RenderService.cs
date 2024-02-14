using Backend.Options;
using Backend.Render;
using Microsoft.Extensions.Options;
using Shared.Schema;

namespace Backend.Services;

public class RenderService
{
    public static readonly Dictionary<string, string> ContainerColor = [];

    private readonly IOptionsMonitor<RenderOptions> _renderOptions;
    private readonly IOptionsMonitor<YardOptions> _yardOptions;

    public RenderService(
        IOptionsMonitor<RenderOptions> renderOptions,
        IOptionsMonitor<YardOptions> yardOptions)
    {
        _renderOptions = renderOptions;
        _yardOptions = yardOptions;
    }

    private int GetCubeNumber() => (int)Math.Ceiling(Math.Sqrt(_yardOptions.CurrentValue.BlockAmount));

    private string CreatePosition(double x, double y, double z, double sizeX, double sizeY, double sizeZ)
        => CreatePosition(x + (sizeX / 2), y + (sizeY / 2), z + (sizeZ / 2));

    private string CreatePosition(double x, double y, double z) => $"{x - _renderOptions.CurrentValue.CenterOffsetX} {y} {z - _renderOptions.CurrentValue.CenterOffsetZ}";

    private string CreateRotation(double x, double y, double z) => $"{x} {y} {z}";

    private (int x, int y, int z) GetLogicalOffset(int blockId)
    {
        var magicNumber = GetCubeNumber();
        var yardOptions = _yardOptions.CurrentValue;

        int logicX = (blockId - 1) % magicNumber;
        int logicZ = (blockId - 1) / magicNumber;

        int offsetX = (logicX * yardOptions.BaysPerBlock) + logicX + 1;
        int offsetY = 0;
        int offsetZ = (logicZ * yardOptions.BaysPerBlock) + logicZ + 1;

        return (offsetX, offsetY, offsetZ);
    }


    private (double x, double y, double z) GetBlockOffset(int blockId)
    {
        var renderOptions = _renderOptions.CurrentValue;
        var yardOptions = _yardOptions.CurrentValue;
        var magicNumber = GetCubeNumber();

        int logicX = (blockId - 1) % magicNumber;
        int logicZ = (blockId - 1) / magicNumber;

        double offsetX = ((logicX + 1) * renderOptions.WalkWayWidth) + (logicX * renderOptions.ContainerWidthX * yardOptions.BaysPerBlock);
        double offsetY = 0;
        double offsetZ = ((logicZ + 1) * renderOptions.WalkWayWidth) + (logicZ * renderOptions.ContainerWidthZ * yardOptions.BaysPerBlock);

        return (offsetX, offsetY, offsetZ);
    }

    public List<FloorRender> GetFloors()
    {
        var renderOptions = _renderOptions.CurrentValue;
        var yardOptions = _yardOptions.CurrentValue;
        var cubeNumber = GetCubeNumber();

        var floors = new List<FloorRender>();

        for (int floorIndex = 0; floorIndex <= cubeNumber; floorIndex++)
        {
            var width = renderOptions.WalkWayWidth;
            var lengthX = (renderOptions.ContainerWidthX * yardOptions.BaysPerBlock * cubeNumber) + ((cubeNumber + 1) * renderOptions.WalkWayWidth);
            var lengthZ = (renderOptions.ContainerWidthZ * yardOptions.StacksPerBlock * cubeNumber) + ((cubeNumber + 1) * renderOptions.WalkWayWidth);

            var offsetX = (floorIndex * renderOptions.WalkWayWidth) + (floorIndex * renderOptions.ContainerWidthX * yardOptions.BaysPerBlock);
            var offsetZ = (floorIndex * renderOptions.WalkWayWidth) + (floorIndex * renderOptions.ContainerWidthZ * yardOptions.StacksPerBlock);

            floors.Add(new()
            {
                XLength = width,
                ZLength = lengthZ,
                Position = CreatePosition(offsetX, 0, 0, width, 0, lengthZ),
                Rotation = CreateRotation(-90, 0, 0),
                Color = "#919191",
            });
            floors.Add(new()
            {
                XLength = lengthX,
                ZLength = width,
                Position = CreatePosition(0, 0, offsetZ, lengthX, 0, width),
                Rotation = CreateRotation(-90, 0, 0),
                Color = "#919191"
            });
        }

        for (int blockIndex = 1; blockIndex <= yardOptions.BlockAmount; blockIndex++)
        {
            var (offsetX, offsetY, offsetZ) = GetBlockOffset(blockIndex);
            var xLength = renderOptions.ContainerWidthX * yardOptions.BaysPerBlock;
            var zLengtn = renderOptions.ContainerWidthZ * yardOptions.StacksPerBlock;

            floors.Add(new()
            {
                XLength = xLength,
                ZLength = zLengtn,
                Position = CreatePosition(offsetX, offsetY, offsetZ, xLength, 0, zLengtn),
                Rotation = CreateRotation(-90, 0, 0),
                Color = "#1bc0f7"
            });
        }

        return floors;
    }

    public List<ContainerRender> GetContainers(List<ContainerSchema> containers)
    {
        var renderOptions = _renderOptions.CurrentValue;

        var renders = new List<ContainerRender>();

        var widthX = renderOptions.ContainerWidthX;
        var widthY = renderOptions.ContainerWidthY;
        var widthZ = renderOptions.ContainerWidthZ;

        foreach (var container in containers)
        {
            var (logicalBlockOffsetX, logicalBlockOffsetY, logicalBlockOffsetZ) = GetLogicalOffset(container.BlockId);
            var (blockOffsetX, blockOffsetY, blockOffsetZ) = GetBlockOffset(container.BlockId);

            var logicalOffsetX = container.BayNum - 1;
            var logicalOffsetY = container.TierNum - 1;
            var logicalOffsetZ = container.StackNum - 1;

            var offsetX = logicalOffsetX * renderOptions.ContainerWidthX;
            var offsetY = logicalOffsetY * renderOptions.ContainerWidthY;
            var offsetZ = logicalOffsetZ * renderOptions.ContainerWidthZ;

            renders.Add(new()
            {
                Id = container.Id,

                XLength = widthX,
                YLength = widthY,
                ZLength = widthZ,
                Position = CreatePosition(blockOffsetX + offsetX,
                    blockOffsetY + offsetY,
                    blockOffsetZ + offsetZ,
                    widthZ / 2,
                    widthY,
                    widthZ / 2),
                Rotation = CreateRotation(0, 0, 0),

                LogicalX = logicalOffsetX + logicalBlockOffsetX,
                LogicalY = logicalOffsetY + logicalBlockOffsetY,
                LogicalZ = logicalOffsetZ + logicalBlockOffsetZ,

                Color = "#d4d4d4",
            });
        }

        return renders;
    }

    public List<TextRender> GetTexts(List<ContainerSchema> containers)
    {
        var renderOptions = _renderOptions.CurrentValue;
        var yardOptions = _yardOptions.CurrentValue;

        var widthX = (double)renderOptions.ContainerWidthX;
        var widthY = (double)renderOptions.ContainerWidthY;
        var widthZ = (double)renderOptions.ContainerWidthZ;

        var texts = new List<TextRender>();

        foreach (var container in containers)
        {
            var (blockOffsetX, blockOffsetY, blockOffsetZ) = GetBlockOffset(container.BlockId);

            var offsetX = (container.BayNum - 1) * renderOptions.ContainerWidthX;
            var offsetY = (container.TierNum - 1) * renderOptions.ContainerWidthY;
            var offsetZ = (container.StackNum - 1) * renderOptions.ContainerWidthZ;

            // front text
            texts.Add(new()
            {
                Text = $"value: {container.Id}; align: center; letterSpacing: -1; wrapCount: 20; color: #000000",
                Position = CreatePosition(blockOffsetX + offsetX,
                    blockOffsetY + offsetY,
                    blockOffsetZ + offsetZ,
                    widthX,
                    widthY,
                    3 * widthZ / 2),
                Rotation = CreateRotation(0, 0, 0),
            });

            // rear text
            texts.Add(new()
            {
                Text = $"value: {container.Id}; align: center; letterSpacing: -1; wrapCount: 20; color: #000000",
                Position = CreatePosition(blockOffsetX + offsetX,
                    blockOffsetY + offsetY,
                    blockOffsetZ + offsetZ,
                    widthX,
                    widthY,
                    -widthZ / 2),
                Rotation = CreateRotation(180, 0, 180),
            });
        }


        for (int blockIndex = 1; blockIndex <= yardOptions.BlockAmount; blockIndex++)
        {
            var (blockOffsetX, blockOffsetY, blockOffsetZ) = GetBlockOffset(blockIndex);

            texts.Add(new()
            {
                Text = $"value: Block #{blockIndex}; align: center; letterSpacing: -1; wrapCount: 20; color: #000000",
                Position = CreatePosition(blockOffsetX + (renderOptions.ContainerWidthX * yardOptions.BaysPerBlock / 2),
                    blockOffsetY,
                    blockOffsetZ + (renderOptions.ContainerWidthZ * yardOptions.StacksPerBlock)),
                Rotation = CreateRotation(-90, 0, 0),
            });

            texts.Add(new()
            {
                Text = $"value: Block #{blockIndex}; align: center; letterSpacing: -1; wrapCount: 20; color: #000000",
                Position = CreatePosition(blockOffsetX + (renderOptions.ContainerWidthX * yardOptions.BaysPerBlock / 2),
                    blockOffsetY,
                    blockOffsetZ),
                Rotation = CreateRotation(-90, 180, 0),
            });
        }

        return texts;
    }

    public List<TableRender> GetTables()
    {
        var yardOptions = _yardOptions.CurrentValue;

        var tables = new List<TableRender>();

        var magicNumber = GetCubeNumber();

        var lengthX = (yardOptions.BaysPerBlock * magicNumber) + magicNumber + 1;
        var lengthZ = (yardOptions.StacksPerBlock * magicNumber) + magicNumber + 1;


        for (int tier = 1; tier <= yardOptions.TiersPerBlock; tier++)
        {
            tables.Add(new()
            {
                Id = "containermap-level-" + tier,
                LevelName = "Level " + tier,
                Level = tier,
                RowCount = lengthX,
                ColumnCount = lengthZ,
            });
        }

        return tables;
    }
}
