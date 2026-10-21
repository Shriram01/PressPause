using System.Security.Claims;
using MediatR;
using PressPause.Application.Users;

namespace PressPause.Api.Endpoints.Users;

public static class UserProfileEndpoints
{
    public static IEndpointRouteBuilder MapUserProfileEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/me/profile")
            .RequireAuthorization()
            .WithTags("User Profiles");

        group.MapGet("/", async (
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var profile = await sender.Send(
                    new GetUserProfileQuery(userId));

                return profile is null
                    ? Results.NotFound()
                    : Results.Ok(profile);
            })
            .WithName("GetUserProfile");

        group.MapPut("/", async (
                UserProfileEndpoints.UpdateUserProfileRequest request,
                ClaimsPrincipal user,
                ISender sender) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                var command = new UpdateUserProfileCommand(
                    userId,
                    request.DisplayName,
                    request.Bio,
                    request.AvatarUrl);

                var updated = await sender.Send(command);

                return updated
                    ? Results.NoContent()
                    : Results.NotFound();
            })
            .WithName("UpdateUserProfile");

        return app;
    }

    public sealed record UpdateUserProfileRequest(
        string DisplayName,
        string? Bio,
        string? AvatarUrl);
}