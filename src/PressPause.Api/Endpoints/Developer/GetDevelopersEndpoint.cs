using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Developers.Queries.GetDevelopers;

namespace PressPause.Api.Endpoints.Developers;

public static class GetDevelopersEndpoint
{
    public static IEndpointRouteBuilder MapGetDevelopersEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/developer",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetDevelopersQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetDevelopers")
            .WithTags("Developers");

        return app;
    }
}