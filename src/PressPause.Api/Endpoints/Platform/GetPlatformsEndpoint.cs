using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Platforms.Queries.GetPlatforms;

namespace PressPause.Api.Endpoints.Platforms;

public static class GetPlatformsEndpoint
{
    public static IEndpointRouteBuilder MapGetPlatformsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/platforms",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetPlatformsQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetPlatforms")
            .WithTags("Platforms");

        return app;
    }
}