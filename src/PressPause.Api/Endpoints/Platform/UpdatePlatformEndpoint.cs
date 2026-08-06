using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Platforms.Commands.UpdatePlatform;

namespace PressPause.Api.Endpoints.Platforms;

public static class UpdatePlatformEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePlatformEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/platforms/{id:guid}",
                async (
                    Guid id,
                    UpdatePlatformCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdatePlatform")
            .WithTags("Platforms");

        return app;
    }
}