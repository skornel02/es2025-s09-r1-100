using Backend;
using Shared.Schema;
using Shared.Utility;

namespace Test.Mocks;

public class MockApplicationDbContext : IApplicationDbContext
{
    public List<ContainerSchema> Containers { get; init; }

    public MockApplicationDbContext(List<ContainerSchema>? containers = null)
    {
        Containers = containers ?? [];
    }

    public Task AddContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default)
    {
        Containers.Add(container);
        return Task.CompletedTask;
    }

    public Task DeleteContainerAsync(string id, CancellationToken cancellationToken = default)
    {
        Containers.RemoveAll(x => x.Id == id);
        return Task.CompletedTask;
    }

    public Task<List<ContainerSchema>> GetAllContainersAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<List<ContainerSchema>>([.. Containers]);
    }

    public Task<DatabaseResponse<ContainerSchema>> GetContainersPaginatedAsync(DatabaseQueryParams param, CancellationToken cancellationToken = default)
    {
        var containersQuery = Containers;

        // this is handled by db, will not be tested.

        var containersMatching = containersQuery.ToList();

        var containersLimited = containersMatching.Skip(param.PageStart).Take(param.PageSize).ToList();

        return Task.FromResult(new DatabaseResponse<ContainerSchema>(containersMatching.Count, containersLimited));
    }
}
