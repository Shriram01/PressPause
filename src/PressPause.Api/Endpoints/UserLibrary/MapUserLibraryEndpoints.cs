using System.Security.Claims;
using MediatR;
using PressPause.Application.Features.UserLibrary.Commands;
using PressPause.Application.Features.UserLibrary.Queries.GetUserLibrary;

namespace PressPause.Api.Endpoints.UserLibrary;

public static class UserLibraryEndpoints
{
    public static IEndpointRouteBuilder MapUserLibraryEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users/me/library")
            .RequireAuthorization()
            .WithTags("User Library");

        group.MapPost("/", async (
            AddUserGameCommand command,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var id = await sender.Send(
                command with { UserId = userId });

            return Results.Created(
                $"/api/users/me/library/{command.GameId}",
                id);
        });

        group.MapGet("/", async (
            int? page,
            int? pageSize,
            string? status,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            PressPause.Domain.Entities.GameStatus? gameStatus = null;

            if (status is not null)
            {
                if (!Enum.TryParse<PressPause.Domain.Entities.GameStatus>(
                        status, true, out var parsedStatus)
                    || !Enum.IsDefined(parsedStatus))
                {
                    return Results.BadRequest(new
                    {
                        message = "Invalid game status."
                    });
                }

                gameStatus = parsedStatus;
            }

            var result = await sender.Send(new GetUserLibraryQuery(
                userId,
                page ?? 1,
                pageSize ?? 20,
                gameStatus));

            return Results.Ok(result);
        });

        group.MapPut("/{gameId:guid}/status", async (
            Guid gameId,
            UpdateStatusRequest request,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await sender.Send(new UpdateUserGameStatusCommand(
                userId,
                gameId,
                request.Status));

            return Results.NoContent();
        });

        group.MapDelete("/{gameId:guid}", async (
            Guid gameId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await sender.Send(new RemoveUserGameCommand(
                userId,
                gameId));

            return Results.NoContent();
        });

        return app;
    }

    public sealed record UpdateStatusRequest(
        PressPause.Domain.Entities.GameStatus Status);
}