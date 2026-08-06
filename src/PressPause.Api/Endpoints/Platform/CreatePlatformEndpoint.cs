using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Platforms.Commands.CreatePlatform;

namespace PressPause.Api.Endpoints.Platforms;

public static class CreatePlatformEndpoint
{
    public static IEndpointRouteBuilder MapCreatePlatformEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/platforms",
                async (
                    CreatePlatformCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/platforms/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreatePlatform")
            .WithTags("Platforms");

        return app;
    }
}