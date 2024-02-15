using Backend.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Importing;
using Shared.Schema;
using Shared.Utility;

namespace Backend.Endpoints;

public static class ContainerEndpoints
{
    public static IEndpointRouteBuilder MapContainerEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("/containers", async Task<Results<Ok<List<ContainerSchema>>, BadRequest>> (
            [FromServices] ApplicationDbContext context,
            CancellationToken cancellationToken
            ) => {
                var containers = await context.GetAllContainersAsync(cancellationToken);

                return TypedResults.Ok(containers);
        })
            .WithTags("Containers");

        builder.MapGet("/ajax/containers", async Task<Results<Ok<AjaxContainer<ContainerSchema>>, BadRequest>> (
            [FromQuery(Name = "draw")] int draw,
            [FromQuery(Name = "start")] int pageStart,
            [FromQuery(Name = "length")] int pageSize,
            [FromQuery(Name = "search[value]")] string globalSearch,
            [FromQuery(Name = "order[0][column]")] int? order1Column,
            [FromQuery(Name = "order[0][dir]")] string? order1Dir,
            [FromQuery(Name = "order[1][column]")] int? order2Column,
            [FromQuery(Name = "order[1][dir]")] string? order2Dir,
            [FromQuery(Name = "order[2][column]")] int? order3Column,
            [FromQuery(Name = "order[2][dir]")] string? order3Dir,
            [FromQuery(Name = "order[3][column]")] int? order4Column,
            [FromQuery(Name = "order[3][dir]")] string? order4Dir,
            [FromQuery(Name = "order[4][column]")] int? order5Column,
            [FromQuery(Name = "order[4][dir]")] string? order5Dir,
            [FromQuery(Name = "order[5][column]")] int? order6Column,
            [FromQuery(Name = "order[5][dir]")] string? order6Dir,
            [FromQuery(Name = "columns[0][search][value]")] string search1,
            [FromQuery(Name = "columns[1][search][value]")] string search2,
            [FromQuery(Name = "columns[2][search][value]")] string search3,
            [FromQuery(Name = "columns[3][search][value]")] string search4,
            [FromQuery(Name = "columns[4][search][value]")] string search5,
            [FromQuery(Name = "columns[5][search][value]")] string search6,
            [FromServices] ApplicationDbContext context,
            CancellationToken cancellationToken
            ) => {
                var globalSearchValue = string.IsNullOrEmpty(globalSearch) ? null : globalSearch;
                var littleSearchValue = new Dictionary<string, string>();
                var sorting = "";
                var ordering = "";
                if (order1Column.HasValue)
                {
                    sorting += ContainerSchema.PropertyOrder[order1Column.Value][..1].ToLower() +
                        ContainerSchema.PropertyOrder[order1Column.Value][1..];
                    ordering += order1Dir == "desc" ? "desc" : "asc";
                }
                if (order2Column.HasValue)
                {
                    sorting += ",";
                    sorting += ContainerSchema.PropertyOrder[order2Column.Value][..1].ToLower() +
                        ContainerSchema.PropertyOrder[order2Column.Value][1..];
                    ordering += ",";
                    ordering += order2Dir == "desc" ? "desc" : "asc";
                }
                if (order3Column.HasValue)
                {
                    sorting += ",";
                    sorting += ContainerSchema.PropertyOrder[order3Column.Value][..1].ToLower() +
                        ContainerSchema.PropertyOrder[order3Column.Value][1..];
                    ordering += ",";
                    ordering += order3Dir == "desc" ? "desc" : "asc";
                }
                if (order4Column.HasValue)
                {
                    sorting += ",";
                    sorting += ContainerSchema.PropertyOrder[order4Column.Value][..1].ToLower() +
                        ContainerSchema.PropertyOrder[order4Column.Value][1..];
                    ordering += ",";
                    ordering += order3Dir == "desc" ? "desc" : "asc";
                }
                if (order5Column.HasValue)
                {
                    sorting += ",";
                    sorting += ContainerSchema.PropertyOrder[order5Column.Value][..1].ToLower() +
                        ContainerSchema.PropertyOrder[order5Column.Value][1..];
                    ordering += ",";
                    ordering += order3Dir == "desc" ? "desc" : "asc";
                }

                if (!string.IsNullOrEmpty(search1)) {
                    littleSearchValue[ContainerSchema.PropertyOrder[0][..1].ToLower() +
                        ContainerSchema.PropertyOrder[0][1..]] = search1;
                }
                if (!string.IsNullOrEmpty(search2))
                {
                    littleSearchValue[ContainerSchema.PropertyOrder[1][..1].ToLower() +
                        ContainerSchema.PropertyOrder[1][1..]] = search2;
                }
                if (!string.IsNullOrEmpty(search3))
                {
                    littleSearchValue[ContainerSchema.PropertyOrder[2][..1].ToLower() +
                        ContainerSchema.PropertyOrder[2][1..]] = search3;
                }
                if (!string.IsNullOrEmpty(search4))
                {
                    littleSearchValue[ContainerSchema.PropertyOrder[3][..1].ToLower() +
                        ContainerSchema.PropertyOrder[3][1..]] = search4;
                }
                if (!string.IsNullOrEmpty(search5))
                {
                    littleSearchValue[ContainerSchema.PropertyOrder[4][..1].ToLower() +
                        ContainerSchema.PropertyOrder[4][1..]] = search5;
                }
                if (!string.IsNullOrEmpty(search6))
                {
                    littleSearchValue[ContainerSchema.PropertyOrder[5][..1].ToLower() +
                        ContainerSchema.PropertyOrder[5][1..]] = search6;
                }

                var containers = await context.GetContainersPaginatedAsync(
                    new(pageStart, 
                        pageSize, 
                        string.IsNullOrEmpty(sorting) ? null : sorting, 
                        string.IsNullOrEmpty(ordering) ? null : ordering, 
                        littleSearchValue,
                        globalSearchValue), 
                    cancellationToken);

                var ajaxResponse = new AjaxContainer<ContainerSchema>(
                    Draw: draw,
                    RecordsTotal: containers.TotalCount,
                    RecordsFiltered: containers.TotalCount,
                    Data: containers.Data
                );
                return TypedResults.Ok(ajaxResponse);
            })
            .WithTags("Containers");

        builder.MapPost("/containers", async Task<Results<Ok<BulkImportResult>, BadRequest>> (
            [FromBody] List<ContainerSchema> containers,
            [FromServices] ContainerService service,
            CancellationToken cancellationToken) =>
        {
            return TypedResults.Ok(await service.ImportContainers(containers, cancellationToken));
        })
            .WithTags("Containers");

        builder.MapPost("/containers/validate", async Task<Ok<List<string>>> (
            [FromServices] ApplicationDbContext context,
            [FromServices] ContainerService service,
            CancellationToken cancellationToken) =>
        {
            var containers = await context.GetAllContainersAsync(cancellationToken);
            var validation = service.ValidateContainers(containers);

            return TypedResults.Ok(validation.Where(_ => _.Value is false)
                .Select(_ => _.Key)
                .ToList());
        })
            .WithTags("Containers");

        return builder;
    }
}
