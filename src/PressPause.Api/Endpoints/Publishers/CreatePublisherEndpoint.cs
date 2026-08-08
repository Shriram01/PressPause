using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Publishers.Commands.CreatePublisher;

namespace PressPause.Api.Endpoints.Publishers;

public static class CreatePublisherEndpoint
{
    public static IEndpointRouteBuilder MapCreatePublisherEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/publishers",
                async (
                    CreatePublisherCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/publishers/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreatePublisher")
            .WithTags("Publishers");

        return app;
    }
}