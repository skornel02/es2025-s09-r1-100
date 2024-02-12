using Backend.Options;
using Microsoft.Extensions.Options;
using Shared.Importing;
using Shared.Schema;

namespace Backend.Services;

public class ContainerService
{
    private readonly ILogger<ContainerService> _logger;
    private readonly IOptionsMonitor<YardOptions> _options;
    private readonly ApplicationDbContext _context;

    public ContainerService(
        ILogger<ContainerService> logger,
        IOptionsMonitor<YardOptions> options,
        ApplicationDbContext context)
    {
        _logger = logger;
        _options = options;
        _context = context;
    }

    public async Task<BulkImportResult> ImportContainers(List<ContainerSchema> containers, CancellationToken cancellationToken = default)
    {
        var existingContainers = await _context.GetContainersAsync(cancellationToken);

        var collidingContainers = FindCollidingContainers(existingContainers, containers);
        var allContainers = containers.Concat(existingContainers).Except(collidingContainers);

        var validation = ValidateContainers(allContainers);

        var validContainers = containers.Where(_ => validation.GetValueOrDefault(_) is true)
            .ToList();

        foreach (var containerToCreate in validContainers)
        {
            await _context.AddContainerAsync(containerToCreate, cancellationToken);
        }

        var invalidContainers = containers.Where(_ => validation.GetValueOrDefault(_) is false)
            .ToList();

        return new BulkImportResult
        {
            Success = validContainers.Count,
            IncorrectPositions = invalidContainers.Concat(collidingContainers)
                .Select(_ => _.Id)
                .ToList()
        };
    }

    private List<ContainerSchema> FindCollidingContainers(IEnumerable<ContainerSchema> existing, IEnumerable<ContainerSchema> containers)
    {
        var colliders = new List<ContainerSchema>();

        foreach (var container in containers)
        {
            var colliding = existing.FirstOrDefault(_ => _.BlockId == container.BlockId
                    && _.StackNum == container.StackNum
                    && _.BayNum == container.BayNum
                    && _.TierNum == container.TierNum);
            if (colliding is not null)
            {
                colliders.Add(container);
                _logger.LogInformation("Container validation for '{id}' failed because it collided with: '{anotherId}'",
                    container.Id,
                    colliding.Id);
            }
        }

        return colliders;
    }

    public Dictionary<ContainerSchema, bool> ValidateContainers(IEnumerable<ContainerSchema> containers)
    {
        var results = new Dictionary<ContainerSchema, bool>();

        foreach (var container in containers)
        {
            var isFloor = container.TierNum == 1;
            var isSupported = isFloor;

            if (!isFloor)
            {
                // TODO: this sucks, floating can happen.
                var containerBellow = containers.FirstOrDefault(_ => _.BlockId == container.BlockId
                    && _.StackNum == container.StackNum
                    && _.BayNum == container.BayNum
                    && _.TierNum == container.TierNum - 1);

                isSupported = containerBellow is not null;
            }

            var options = _options.CurrentValue;
            var isWithinBounds = container.BlockId <= options.BlockAmount && container.BlockId > 0
                && container.BayNum <= options.BaysPerBlock && container.BayNum > 0
                && container.StackNum <= options.StacksPerBlock && container.StackNum > 0
                && container.TierNum <= options.TiersPerBlock && options.TiersPerBlock > 0;

            _logger.LogInformation("Container validation for: '{id}' results: [supported='{supported}' bounds='{in-area}']",
                container.Id,
                isFloor || isSupported,
                isWithinBounds);

            results[container] = (isFloor || isSupported) && isWithinBounds;
        }

        return results;
    }
}
