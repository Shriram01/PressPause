using MediatR;
using Microsoft.AspNetCore.Mvc;
using PressPause.Application.Common.Models;
using PressPause.Application.Games.Queries.GetGames;

namespace PressPause.Api.Endpoints.Games;

public static class GetGamesEndpoint
{
    public static IEndpointRouteBuilder MapGetGamesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/games",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetGamesQuery(
                            pagination.Page,
                            pagination.PageSize));

                    return Results.Ok(result);
                })
            .WithName("GetGames")
            .WithTags("Games");

        return app;
    }
}