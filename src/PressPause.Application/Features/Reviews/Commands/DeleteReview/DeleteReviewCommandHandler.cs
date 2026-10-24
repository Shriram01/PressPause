using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed class DeleteReviewCommandHandler(
    IReviewRepository reviewRepository)
    : IRequestHandler<DeleteReviewCommand>
{
    public async Task Handle(
        DeleteReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(
            request.ReviewId,
            cancellationToken);

        if (review is null || review.UserId != request.UserId)
            throw new KeyNotFoundException("Review was not found.");

        await reviewRepository.DeleteAsync(review, cancellationToken);
        await reviewRepository.SaveChangesAsync(cancellationToken);
    }
}