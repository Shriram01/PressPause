using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Tags.Queries.GetTags;

namespace PressPause.Api.Endpoints.Tags;

public static class GetTagsEndpoint
{
    public static IEndpointRouteBuilder MapGetTagsEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetTagsQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetTags")
            .WithTags("Tags");

        return app;
    }
}