using MediatR;
using PressPause.Application.Games.Commands.DeleteGame;

namespace PressPause.Api.Endpoints.Games;

public static class DeleteGameEndpoint
{
    public static IEndpointRouteBuilder MapDeleteGameEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/games/{id:guid}",
                async (Guid id, ISender sender) =>
                {
                    await sender.Send(new DeleteGameCommand(id));

                    return Results.NoContent();
                })
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithName("DeleteGame")
            .WithTags("Games");

        return app;
    }
}