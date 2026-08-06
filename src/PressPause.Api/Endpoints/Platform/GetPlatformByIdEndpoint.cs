using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Platforms.Queries.GetPlatformById;

namespace PressPause.Api.Endpoints.Platforms;

public static class GetPlatformByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetPlatformByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/platforms/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetPlatformByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetPlatformById")
            .WithTags("Platforms");

        return app;
    }
}