using MediatR;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed record UpdateGameCommand(
    Guid Id,
    string Title,
    string Slug,
    DateOnly? ReleaseDate) : IRequest;