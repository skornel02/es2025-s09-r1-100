using Backend.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Importing;

namespace Backend.Endpoints;

public static class ImportEndpoints
{
    public static IEndpointRouteBuilder MapImportEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("/containers/import", async Task<Results<Ok<BulkImportResult>, BadRequest>> (
            IFormFile csv) =>
        {
            var result = new BulkImportResult();

            var parsedCsv = await csv.ParseCsvAsync();
            var containers = parsedCsv.ParseContainers();

            return TypedResults.Ok(result);
        })
            .WithTags("Container import")
            .DisableAntiforgery();

        return builder;
    }
}
