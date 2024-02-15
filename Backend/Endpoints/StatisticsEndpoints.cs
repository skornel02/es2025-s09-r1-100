using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Statistics;
using System.Threading;

namespace Backend.Endpoints;

public static class StatisticsEndpoints
{

    public static IEndpointRouteBuilder MapStatisticsEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("blocks/stat", async Task<Results<Ok<List<BlockStatistics>>, BadRequest>> (
            [FromServices] ApplicationDbContext context,
            [FromServices] StatisticsService statisticsService,
            CancellationToken cancellationToken
            ) =>
        {
            var containers = await context.GetAllContainersAsync(cancellationToken);

            return TypedResults.Ok(statisticsService.GetStatistics(containers));
        })
            .WithTags("Statistics");

        return builder;
    }
}
