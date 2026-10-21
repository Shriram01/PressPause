using System.Security.Claims;

namespace PressPause.Api.Endpoints.Users;

public static class GetCurrentUserEndpoint
{
    public static IEndpointRouteBuilder MapGetCurrentUserEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/users/me", (ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = user.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrWhiteSpace(userId))
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(new
                {
                    Id = userId,
                    Email = email
                });
            })
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithTags("Users");

        return app;
    }
}