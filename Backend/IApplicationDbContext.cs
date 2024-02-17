using Shared.Schema;
using Shared.Utility;

namespace Backend;

public interface IApplicationDbContext
{
    public Task<DatabaseResponse<ContainerSchema>> GetContainersPaginatedAsync(
        DatabaseQueryParams param,
        CancellationToken cancellationToken = default);

    public Task<List<ContainerSchema>> GetAllContainersAsync(CancellationToken cancellationToken = default);

    public Task AddContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default);

    public Task DeleteContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default)
    {
        return DeleteContainerAsync(container.Id, cancellationToken);
    }

    public Task DeleteContainerAsync(string id, CancellationToken cancellationToken = default);
}
