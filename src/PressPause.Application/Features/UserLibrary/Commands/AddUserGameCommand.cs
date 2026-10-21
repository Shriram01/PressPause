using MediatR;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed record AddUserGameCommand(
    string UserId,
    Guid GameId) : IRequest<Guid>;