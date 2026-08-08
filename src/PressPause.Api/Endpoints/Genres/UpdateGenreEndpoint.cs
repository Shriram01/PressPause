using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Genres.Commands.UpdateGenre;

namespace PressPause.Api.Endpoints.Genres;

public static class UpdateGenreEndpoint
{
    public static IEndpointRouteBuilder MapUpdateGenreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/genres/{id:guid}",
                async (
                    Guid id,
                    UpdateGenreCommand command,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        command with { Id = id });

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("UpdateGenres")
            .WithTags("Genres");

        return app;
    }
}