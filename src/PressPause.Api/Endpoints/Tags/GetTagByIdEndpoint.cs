using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Tags.Queries.GetTagById;

namespace PressPause.Api.Endpoints.Tags;

public static class GetTagByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetTagByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tags/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetTagByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetTagById")
            .WithTags("Tags");

        return app;
    }
}