using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Genres.Queries.GetGenres;

namespace PressPause.Api.Endpoints.Genres;

public static class GetGenresEndpoint
{
    public static IEndpointRouteBuilder MapGetGenresEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/genres",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetGenresQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetGenres")
            .WithTags("Genres");

        return app;
    }
}