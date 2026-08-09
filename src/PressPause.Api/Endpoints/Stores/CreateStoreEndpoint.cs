using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Stores.Commands.CreateStore;

namespace PressPause.Api.Endpoints.Stores;

public static class CreateStoreEndpoint
{
    public static IEndpointRouteBuilder MapCreateStoreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/stores",
                async (
                    CreateStoreCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/stores/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreateStore")
            .WithTags("Stores");

        return app;
    }
}