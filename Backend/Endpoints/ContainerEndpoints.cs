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
            [FromBody] List<ContainerSchema> containers) =>
        {
            var result = new BulkImportResult();

            return TypedResults.Ok(result);
        })
            .WithTags("Containers");

        return builder;
    }
}
