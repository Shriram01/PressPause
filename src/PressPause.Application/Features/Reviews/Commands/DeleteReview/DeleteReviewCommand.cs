using MediatR;

namespace PressPause.Application.Features.Reviews.Commands;

public sealed record DeleteReviewCommand(
    string UserId,
    Guid ReviewId) : IRequest;