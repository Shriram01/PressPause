using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Publishers.Commands.DeletePublisher;

namespace PressPause.Api.Endpoints.Publishers;

public static class DeletePublisherEndpoint
{
    public static IEndpointRouteBuilder MapDeletePublisherEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/publishers/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeletePublisherCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeletePublisher")
            .WithTags("Publishers");

        return app;
    }
}