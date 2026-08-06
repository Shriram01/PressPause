using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Genres.Commands.CreateGenre;

namespace PressPause.Api.Endpoints.Genres;

public static class CreateGenreEndpoint
{
    public static IEndpointRouteBuilder MapCreateGenreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/genres",
                async (
                    CreateGenreCommand command,
                    ISender sender) =>
                {
                    var id = await sender.Send(command);

                    return Results.Created(
                        $"/api/genres/{id}",
                        new { Id = id });
                })
            .RequireAuthorization()
            .WithName("CreateGenre")
            .WithTags("Genres");

        return app;
    }
}