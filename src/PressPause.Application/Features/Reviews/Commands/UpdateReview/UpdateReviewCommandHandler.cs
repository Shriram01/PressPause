using MediatR;
using PressPause.Application.Common.Exceptions;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed class UpdateReviewCommandHandler(
    IReviewRepository reviewRepository)
    : IRequestHandler<UpdateReviewCommand>
{
    public async Task Handle(
        UpdateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var review = await reviewRepository.GetByIdAsync(
            request.ReviewId,
            cancellationToken);

        if (review is null)
            throw new KeyNotFoundException("Review was not found.");

        if (review.UserId != request.UserId)
            throw new UnauthorizedAccessException(
                "You can only update your own reviews.");

        review.Update(request.Rating, request.Text);

        await reviewRepository.SaveChangesAsync(cancellationToken);
    }
}