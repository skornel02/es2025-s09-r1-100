using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Importing;

namespace Backend.Endpoints;

public static class ImportEndpoints
{
    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/containers/import", async Task<Results<Ok<BulkImportResult>, BadRequest>> (
            IFormFile csv,
            [FromServices] ContainerService service,
            CancellationToken cancellationToken) =>
        {
            var parsedCsv = await csv.ParseCsvAsync(cancellationToken: cancellationToken);
            var containers = parsedCsv.ParseContainers();

            return TypedResults.Ok(await service.ImportContainers(containers));
        })
            .WithTags("Container import")
            .DisableAntiforgery();

        return builder;
    }
}
