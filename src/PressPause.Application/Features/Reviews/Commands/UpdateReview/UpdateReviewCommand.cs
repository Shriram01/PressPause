using MediatR;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed record UpdateReviewCommand(
    string UserId,
    Guid ReviewId,
    int Rating,
    string? Text) : IRequest;