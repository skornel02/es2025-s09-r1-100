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
        var existingContainers = await _context.GetAllContainersAsync(cancellationToken);

        var duplicateIdContainers = containers.Where(_ => existingContainers.Any(__ => __.Id == _.Id))
            .ToList();

        if (duplicateIdContainers.Any())
        {
            _logger.LogWarning("Tried to import duplicate ids: [{ids}]", string.Join(", ", duplicateIdContainers.Select(_ => _.Id)));
        }

        var collidingContainers = FindCollidingContainers(existingContainers, containers.Except(duplicateIdContainers));

        var allContainers = containers
            .Except(collidingContainers)
            .Except(duplicateIdContainers)
            .Concat(existingContainers)
            .ToList();

        var validation = ValidateContainers(allContainers);

        var validContainers = containers
            .Except(collidingContainers)
            .Except(duplicateIdContainers)
            .Where(_ => validation.GetValueOrDefault(_.Id) is true)
            .ToList();

        foreach (var containerToCreate in validContainers)
        {
            await _context.AddContainerAsync(containerToCreate, cancellationToken);
        }

        var validationFailedContainers = containers.Where(_ => validation.ContainsKey(_.Id) && validation[_.Id] is false).ToList();
        var invalidContainers = validationFailedContainers
            .Concat(collidingContainers)
            .Concat(duplicateIdContainers)
            .ToList();

        return new BulkImportResult
        {
            Success = validContainers.Count,
            IncorrectPositions = invalidContainers
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

    public Dictionary<string, bool> ValidateContainers(List<ContainerSchema> containers)
    {
        var results = new Dictionary<string, bool>();
        var options = _options.CurrentValue;

        foreach (var container in containers.OrderBy(_ => _.TierNum))
        {
            var isFloor = container.TierNum == 1;
            var isSupported = isFloor;

            if (!isFloor)
            {
                var containerBellow = containers.FirstOrDefault(_ => _.BlockId == container.BlockId
                    && _.StackNum == container.StackNum
                    && _.BayNum == container.BayNum
                    && _.TierNum == container.TierNum - 1);
                var bellowSupported = containerBellow is null || results[containerBellow.Id];

                isSupported = containerBellow is not null && bellowSupported;
            }

            var isWithinBounds = container.BlockId <= options.BlockAmount && container.BlockId > 0
                && container.BayNum <= options.BaysPerBlock && container.BayNum > 0
                && container.StackNum <= options.StacksPerBlock && container.StackNum > 0
                && container.TierNum <= options.TiersPerBlock && options.TiersPerBlock > 0;

            _logger.LogInformation("Container validation for: '{id}' results: [supported='{supported}' bounds='{in-area}']",
                container.Id,
                isFloor || isSupported,
                isWithinBounds);

            results[container.Id] = (isFloor || isSupported) && isWithinBounds;
        }

        return results;
    }
}
