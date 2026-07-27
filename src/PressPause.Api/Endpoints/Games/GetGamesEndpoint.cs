using MediatR;
using PressPause.Application.Games.Queries.GetGames;

namespace PressPause.Api.Endpoints.Games;

public static class GetGamesEndpoint
{
    public static IEndpointRouteBuilder MapGetGamesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/games", async (ISender sender) =>
        {
            var games = await sender.Send(new GetGamesQuery());

            return Results.Ok(games);
        });

        return app;
    }
}