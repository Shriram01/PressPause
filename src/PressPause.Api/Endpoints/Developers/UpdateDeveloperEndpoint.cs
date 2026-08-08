using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Developers.Commands.UpdateDeveloper;

namespace PressPause.Api.Endpoints.Developers;

public static class UpdateDeveloperEndpoint
{
    public static IEndpointRouteBuilder MapUpdateDeveloperEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/developers/{id:guid}",
                async (
                    Guid id,
                    UpdateDeveloperCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdateDeveloper")
            .WithTags("Developers");

        return app;
    }
}