using MediatR;
using PressPause.Application.Authentication.Commands.Login;

namespace PressPause.Api.Endpoints.Authentication;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login",
                async (
                    LoginCommand command,
                    ISender sender) =>
                {
                    var token = await sender.Send(command);

                    if (token is null)
                    {
                        return Results.Unauthorized();
                    }

                    return Results.Ok(new
                    {
                        Token = token
                    });
                })
            .WithName("Login")
            .WithTags("Authentication");

        return app;
    }
}