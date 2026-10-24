using MediatR;
using PressPause.Domain.Entities;

namespace PressPause.Application.Features.Reviews.Queries;

public sealed record GetGameReviewsQuery(
    Guid GameId,
    int Page = 1,
    int PageSize = 20)
    : IRequest<GetGameReviewsResult>;

public sealed record GetGameReviewsResult(
    IReadOnlyList<Review> Items,
    int TotalCount,
    int Page,
    int PageSize);