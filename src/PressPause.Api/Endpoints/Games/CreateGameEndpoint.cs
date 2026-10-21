using MediatR;
using PressPause.Application.Games.Commands.CreateGame;

namespace PressPause.Api.Endpoints.Games;

public static class CreateGameEndpoint
{
    public static IEndpointRouteBuilder MapCreateGameEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/games",
                async (CreateGameCommand command, ISender sender) =>
                {
                    Guid id = await sender.Send(command);

                    return Results.Created($"/api/games/{id}", id);
                })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("CreateGame")
            .WithTags("Games");

        return app;
    }
}