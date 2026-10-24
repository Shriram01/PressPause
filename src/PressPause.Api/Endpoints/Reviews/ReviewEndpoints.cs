using System.Security.Claims;
using MediatR;
using PressPause.Application.Features.Reviews.Commands;
using PressPause.Application.Features.Reviews.Queries;

namespace PressPause.Api.Endpoints.Reviews;

public static class ReviewEndpoints
{
    public static IEndpointRouteBuilder MapReviewEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/games/{gameId:guid}/reviews")
            .RequireAuthorization()
            .WithTags("Reviews");

        group.MapPost("/", async (
            Guid gameId,
            CreateReviewRequest request,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var reviewId = await sender.Send(
                new CreateReviewCommand(
                    userId,
                    gameId,
                    request.Rating,
                    request.Text));

            return Results.Created(
                $"/api/reviews/{reviewId}",
                new { id = reviewId });
        });

        group.MapGet("/", async (
                Guid gameId,
                int page,
                int pageSize,
                ISender sender) =>
            {
                page = page < 1 ? 1 : page;
                pageSize = pageSize < 1 ? 20 : Math.Min(pageSize, 100);

                var result = await sender.Send(
                    new GetGameReviewsQuery(gameId, page, pageSize));

                return Results.Ok(result);
            })
            .AllowAnonymous();

        group.MapPut("/{reviewId:guid}", async (
            Guid reviewId,
            UpdateReviewRequest request,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await sender.Send(new UpdateReviewCommand(
                userId, reviewId, request.Rating, request.Text));

            return Results.NoContent();
        });

        group.MapDelete("/{reviewId:guid}", async (
            Guid reviewId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            await sender.Send(new DeleteReviewCommand(userId, reviewId));

            return Results.NoContent();
        });

        return app;
    }

    public sealed record UpdateReviewRequest(int Rating, string? Text);

    public sealed record CreateReviewRequest(
        int Rating,
        string? Text);
}