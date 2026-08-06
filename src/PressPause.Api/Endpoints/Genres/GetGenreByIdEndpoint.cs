using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Genres.Queries.GetGenreById;

namespace PressPause.Api.Endpoints.Genres;

public static class GetGenreByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetGenreByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/genres/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetGenreByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetGenreById")
            .WithTags("Genres");

        return app;
    }
}