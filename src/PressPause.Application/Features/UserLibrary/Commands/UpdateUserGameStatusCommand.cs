using MediatR;
using PressPause.Domain.Entities;

namespace PressPause.Application.Features.UserLibrary.Commands;

public sealed record UpdateUserGameStatusCommand(
    string UserId,
    Guid GameId,
    GameStatus Status) : IRequest;