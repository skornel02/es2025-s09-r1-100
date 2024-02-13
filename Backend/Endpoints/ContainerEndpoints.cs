using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Importing;
using Shared.Schema;

namespace Backend.Endpoints;

public static class ContainerEndpoints
{
    public static IEndpointRouteBuilder MapContainerEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/containers", async Task<Results<Ok<BulkImportResult>, BadRequest>> (
            [FromBody] List<ContainerSchema> containers,
            [FromServices] ContainerService service,
            CancellationToken cancellationToken) =>
        {
            return TypedResults.Ok(await service.ImportContainers(containers));
        })
            .WithTags("Containers");

        builder.MapPost("/containers/validate", async Task<Ok<List<string>>> (
            [FromServices] ApplicationDbContext context,
            [FromServices] ContainerService service) =>
        {
            var containers = await context.GetContainersAsync();
            var validation = service.ValidateContainers(containers.ToList());

            return TypedResults.Ok(validation.Where(_ => _.Value is false)
                .Select(_ => _.Key)
                .ToList());
        })
            .WithTags("Containers");

        return builder;
    }
}
