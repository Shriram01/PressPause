using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Tags.Commands.UpdateTag;

namespace PressPause.Api.Endpoints.Tags;

public static class UpdateTagEndpoint
{
    public static IEndpointRouteBuilder MapUpdateTagEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/tags/{id:guid}",
                async (
                    Guid id,
                    UpdateTagCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdateTag")
            .WithTags("Tags");

        return app;
    }
}