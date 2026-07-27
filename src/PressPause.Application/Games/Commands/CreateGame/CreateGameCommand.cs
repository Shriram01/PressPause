using MediatR;

namespace PressPause.Application.Games.Commands.CreateGame;

public sealed record CreateGameCommand(
    string Title,
    string Slug,
    DateOnly? ReleaseDate) : IRequest<Guid>;