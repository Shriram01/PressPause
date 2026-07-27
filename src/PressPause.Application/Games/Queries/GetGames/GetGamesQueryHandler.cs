using MediatR;
using PressPause.Application.Common.Interfaces;

namespace PressPause.Application.Games.Queries.GetGames;

public sealed class GetGamesQueryHandler
    : IRequestHandler<GetGamesQuery, IReadOnlyList<GameDto>>
{
    private readonly IGameRepository _repository;

    public GetGamesQueryHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<GameDto>> Handle(
        GetGamesQuery request,
        CancellationToken cancellationToken)
    {
        var games = await _repository.GetAllAsync(cancellationToken);

        return games
            .Select(game => new GameDto(
                game.Id,
                game.Title,
                game.Slug,
                game.ReleaseDate))
            .ToList();
    }
}