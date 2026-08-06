using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Genres.Commands.DeleteGenre;

namespace PressPause.Api.Endpoints.Genres;

public static class DeleteGenreEndpoint
{
    public static IEndpointRouteBuilder MapDeleteGenreEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/genres/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var success = await sender.Send(
                        new DeleteGenreCommand(id));

                    return success
                        ? Results.NoContent()
                        : Results.NotFound();
                })
            .RequireAuthorization()
            .WithName("DeleteGenre")
            .WithTags("Genres");

        return app;
    }
}