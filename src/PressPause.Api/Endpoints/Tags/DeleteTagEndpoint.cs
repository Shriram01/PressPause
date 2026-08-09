using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Tags.Commands.DeleteTag;

namespace PressPause.Api.Endpoints.Tags;

public static class DeleteTagEndpoint
{
    public static IEndpointRouteBuilder MapDeleteTagEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/tags/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeleteTagCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeleteTag")
            .WithTags("Tags");

        return app;
    }
}