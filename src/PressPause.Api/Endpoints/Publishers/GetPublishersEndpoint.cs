using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Common.Models;
using PressPause.Application.Publishers.Queries.GetPublishers;

namespace PressPause.Api.Endpoints.Publishers;

public static class GetPublishersEndpoint
{
    public static IEndpointRouteBuilder MapGetPublishersEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/publishers",
                async (
                    [AsParameters] PaginationRequest pagination,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetPublishersQuery(
                            pagination.Page,
                            pagination.PageSize,
                            pagination.Search,
                            pagination.SortBy,
                            pagination.Descending));

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetPublishers")
            .WithTags("Publishers");

        return app;
    }
}