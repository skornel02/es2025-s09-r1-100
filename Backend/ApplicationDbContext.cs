using Shared.Schema;
using System.Net;

namespace Backend;

public class ApplicationDbContext
{
    public readonly HttpClient _httpClient;

    public ApplicationDbContext(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("DatabaseClient");
        _httpClient.BaseAddress = new Uri("http://localhost:3000");
    }

    public async Task<IQueryable<ContainerSchema>> GetContainersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/containers", cancellationToken);
        var data = await response.Content.ReadFromJsonAsync<List<ContainerSchema>>(cancellationToken);

        return (data ?? []).AsQueryable();
    }

    public async Task AddContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsJsonAsync("/containers", container, cancellationToken);
    }

    public async Task DeleteContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default)
    {
        await DeleteContainerAsync(container.Id, cancellationToken);
    }

    public async Task DeleteContainerAsync(string id, CancellationToken cancellationToken = default)
    {
        await _httpClient.DeleteAsync("/containers/" + WebUtility.UrlEncode(id), cancellationToken);
    }
}
