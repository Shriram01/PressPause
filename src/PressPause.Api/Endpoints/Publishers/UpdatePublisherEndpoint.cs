using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Publishers.Commands.UpdatePublisher;

namespace PressPause.Api.Endpoints.Publishers;

public static class UpdatePublisherEndpoint
{
    public static IEndpointRouteBuilder MapUpdatePublisherEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/publishers/{id:guid}",
                async (
                    Guid id,
                    UpdatePublisherCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdatePublisher")
            .WithTags("Publishers");

        return app;
    }
}