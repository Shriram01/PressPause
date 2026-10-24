using MediatR;
using PressPause.Application.Common.Exceptions;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed class CreateReviewCommandHandler(
    IReviewRepository reviewRepository,
    IGameRepository gameRepository)
    : IRequestHandler<CreateReviewCommand, Guid>
{
    public async Task<Guid> Handle(
        CreateReviewCommand request,
        CancellationToken cancellationToken)
    {
        var game = await gameRepository.GetByIdAsync(
            request.GameId,
            cancellationToken);

        if (game is null)
            throw new KeyNotFoundException(
                $"Game '{request.GameId}' was not found.");

        var existingReview =
            await reviewRepository.GetByUserAndGameAsync(
                request.UserId,
                request.GameId,
                cancellationToken);

        if (existingReview is not null)
            throw new ConflictException(
                "You have already reviewed this game.");

        var review = new Review(
            request.UserId,
            request.GameId,
            request.Rating,
            request.Text);

        await reviewRepository.AddAsync(
            review,
            cancellationToken);

        await reviewRepository.SaveChangesAsync(
            cancellationToken);

        return review.Id;
    }
}