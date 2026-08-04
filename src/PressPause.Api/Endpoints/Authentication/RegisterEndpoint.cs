using MediatR;
using PressPause.Application.Authentication.Commands.Register;


namespace PressPause.Api.Endpoints.Authentication;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register",
                async (
                    RegisterCommand command,
                    ISender sender) =>
                {
                    var result = await sender.Send(command);

                    if (!result.Succeeded)
                    {
                        return Results.BadRequest(result.Errors);
                    }

                    return Results.Created();
                })
            .WithName("Register")
            .WithTags("Authentication");

        return app;
    }
}