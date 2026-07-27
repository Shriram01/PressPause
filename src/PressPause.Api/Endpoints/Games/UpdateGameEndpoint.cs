using MediatR;
using PressPause.Application.Games.Commands.UpdateGame;

namespace PressPause.Api.Endpoints.Games;

public static class UpdateGameEndpoint
{
    public static IEndpointRouteBuilder MapUpdateGameEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/games/{id:guid}",
                async (Guid id, UpdateGameCommand command, ISender sender) =>
                {
                    if (id != command.Id)
                    {
                        return Results.BadRequest("Route id and request id must match.");
                    }

                    await sender.Send(command);

                    return Results.NoContent();
                })
            .WithName("UpdateGame")
            .WithTags("Games");

        return app;
    }
}