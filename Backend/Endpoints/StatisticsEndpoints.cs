using Microsoft.AspNetCore.Http.HttpResults;
using Shared.Statistics;

namespace Backend.Endpoints;

public static class StatisticsEndpoints
{

    public static IEndpointRouteBuilder MapStatisticsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("blocks/stat", async Task<Results<Ok<List<BlockStatistics>>, BadRequest>> () =>
        {
            return TypedResults.Ok(new List<BlockStatistics>());
        })
            .WithTags("Statistics");

        return builder;
    }
}
