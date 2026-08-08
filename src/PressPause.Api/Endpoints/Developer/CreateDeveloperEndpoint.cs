using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Developers.Commands.CreateDeveloper;

namespace PressPause.Api.Endpoints.Developers;

public static class CreateDeveloperEndpoint
{
    public static IEndpointRouteBuilder MapCreateDeveloperEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/developer",
                async (
                    CreateDeveloperCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/developer/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreateDeveloper")
            .WithTags("Developers");

        return app;
    }
}