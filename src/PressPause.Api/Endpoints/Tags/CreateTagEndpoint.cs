using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Tags.Commands.CreateTag;

namespace PressPause.Api.Endpoints.Tags;

public static class CreateTagEndpoint
{
    public static IEndpointRouteBuilder MapCreateTagEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tags",
                async (
                    CreateTagCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/tags/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreateTag")
            .WithTags("Tags");

        return app;
    }
}