using MediatR;

namespace PressPause.Application.Games.Commands.DeleteGame;

public sealed record DeleteGameCommand(Guid Id) : IRequest;