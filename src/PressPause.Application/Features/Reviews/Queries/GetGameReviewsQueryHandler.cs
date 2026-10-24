using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Features.Reviews.Queries;

public sealed class GetGameReviewsQueryHandler(
    IReviewRepository reviewRepository)
    : IRequestHandler<GetGameReviewsQuery, GetGameReviewsResult>
{
    public async Task<GetGameReviewsResult> Handle(
        GetGameReviewsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await reviewRepository.GetPagedByGameAsync(
                request.GameId,
                request.Page,
                request.PageSize,
                cancellationToken);

        return new GetGameReviewsResult(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}