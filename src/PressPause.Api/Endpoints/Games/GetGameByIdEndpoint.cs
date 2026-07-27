using MediatR;
using PressPause.Application.Games.Queries.GetGameById;

namespace PressPause.Api.Endpoints.Games;

public static class GetGameByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetGameByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/games/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    var game = await sender.Send(new GetGameByIdQuery(id));

                    return game is null
                        ? Results.NotFound()
                        : Results.Ok(game);
                })
            .WithName("GetGameById")
            .WithTags("Games");

        return app;
    }
}