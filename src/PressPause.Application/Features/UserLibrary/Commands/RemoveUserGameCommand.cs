using MediatR;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed record RemoveUserGameCommand(
    string UserId,
    Guid GameId) : IRequest;