using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Stores.Queries.GetStores;

namespace PressPause.Api.Endpoints.Stores;

public static class GetStoresEndpoint
{
    public static IEndpointRouteBuilder MapGetStoresEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/stores",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetStoresQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetStores")
            .WithTags("Stores");

        return app;
    }
}