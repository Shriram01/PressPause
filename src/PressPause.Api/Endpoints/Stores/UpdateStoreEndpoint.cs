using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Stores.Commands.UpdateStore;

namespace PressPause.Api.Endpoints.Stores;

public static class UpdateStoreEndpoint
{
    public static IEndpointRouteBuilder MapUpdateStoreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/stores/{id:guid}",
                async (
                    Guid id,
                    UpdateStoreCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdateStore")
            .WithTags("Stores");

        return app;
    }
}