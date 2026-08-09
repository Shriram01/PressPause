using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Stores.Commands.DeleteStore;

namespace PressPause.Api.Endpoints.Stores;

public static class DeleteStoreEndpoint
{
    public static IEndpointRouteBuilder MapDeleteStoreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/stores/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeleteStoreCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeleteStore")
            .WithTags("Stores");

        return app;
    }
}