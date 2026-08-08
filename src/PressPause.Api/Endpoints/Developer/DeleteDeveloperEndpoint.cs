using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Developers.Commands.DeleteDeveloper;

namespace PressPause.Api.Endpoints.Developers;

public static class DeleteDeveloperEndpoint
{
    public static IEndpointRouteBuilder MapDeleteDeveloperEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/developer/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeleteDeveloperCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeleteDeveloper")
            .WithTags("Developers");

        return app;
    }
}