using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Platforms.Commands.DeletePlatform;

namespace PressPause.Api.Endpoints.Platforms;

public static class DeletePlatformEndpoint
{
    public static IEndpointRouteBuilder MapDeletePlatformEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/platforms/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeletePlatformCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeletePlatform")
            .WithTags("Platforms");

        return app;
    }
}