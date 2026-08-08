using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Publishers.Queries.GetPublisherById;

namespace PressPause.Api.Endpoints.Publishers;

public static class GetPublisherByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPublisherByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/publishers/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetPublisherByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetPublisherById")
            .WithTags("Publishers");

        return app;
    }
}