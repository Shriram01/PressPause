using MediatR;

namespace PressPause.Application.Games.Queries.GetGames;

public sealed record GetGamesQuery : IRequest<IReadOnlyList<GameDto>>;