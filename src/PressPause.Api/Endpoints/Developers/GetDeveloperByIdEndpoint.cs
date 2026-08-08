using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using PressPause.Application.Developers.Queries.GetDeveloperById;

namespace PressPause.Api.Endpoints.Developers;

public static class GetDeveloperByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetDeveloperByIdEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/developers/{id:guid}",
                async (
                    Guid id,
                    ISender sender) =>
                {
                    var result = await sender.Send(
                        new GetDeveloperByIdQuery(id));

                    if (result is null)
                    {
                        return Results.NotFound();
                    }

                    return Results.Ok(result);
                })
            .RequireAuthorization()
            .WithName("GetDeveloperById")
            .WithTags("Developers");

        return app;
    }
}