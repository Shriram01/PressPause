using MediatR;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed record CreateReviewCommand(
    string UserId,
    Guid GameId,
    int Rating,
    string? Text) : IRequest<Guid>;