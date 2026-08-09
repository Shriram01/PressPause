using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Stores.Queries.GetStoreById;

namespace PressPause.Api.Endpoints.Stores;

public static class GetStoreByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetStoreByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/stores/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetStoreByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetStoreById")
            .WithTags("Stores");

        return app;
    }
}