using Shared.Schema;
using Shared.Utility;
using System.Net;

namespace Backend;

public class ApplicationDbContext : IApplicationDbContext
{
    public readonly HttpClient _httpClient;

    public ApplicationDbContext(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("DatabaseClient");
        _httpClient.BaseAddress = new Uri("http://localhost:3000");
    }

    public async Task<DatabaseResponse<ContainerSchema>> GetContainersPaginatedAsync(
        DatabaseQueryParams param,
        CancellationToken cancellationToken = default)
    {
        var globalSearch = param.GlobalSearch is null ? "" : $"&q=" + WebUtility.UrlEncode(param.GlobalSearch);
        var sort = param.Sorting is null ? "" : "&_sort=" + param.Sorting;
        var ordering = param.Ordering is null ? "" : "&_order=" + param.Ordering;
        var search = string.Join("&", param.Search.Select(_ => $"{_.Key}_like={WebUtility.UrlEncode(_.Value)}"));
        if (!string.IsNullOrEmpty(search))
        {
            search = "&" + search;
        }

        var response = await _httpClient.GetAsync($"/containers?_start={param.PageStart}" +
            $"&_limit={param.PageSize}{globalSearch}{sort}{ordering}{search}", cancellationToken);
        var totalCount = int.TryParse(response.Headers.GetValues("X-Total-Count").FirstOrDefault(), out int count) ? count : 0;
        var data = await response.Content.ReadFromJsonAsync<List<ContainerSchema>>(cancellationToken);

        return new(totalCount, data ?? []);
    }

    public async Task<List<ContainerSchema>> GetAllContainersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/containers", cancellationToken);
        var data = await response.Content.ReadFromJsonAsync<List<ContainerSchema>>(cancellationToken);

        return data ?? [];
    }

    public async Task AddContainerAsync(ContainerSchema container, CancellationToken cancellationToken = default)
    {
        await _httpClient.PostAsJsonAsync("/containers", container, cancellationToken);
    }

    public async Task DeleteContainerAsync(string id, CancellationToken cancellationToken = default)
    {
        await _httpClient.DeleteAsync("/containers/" + WebUtility.UrlEncode(id), cancellationToken);
    }
}
