using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Models;

namespace PressPause.Application.Games.Queries.GetGames;

public sealed class GetGamesQueryHandler
    : IRequestHandler<GetGamesQuery, PagedResult<GameDto>>
{
    private readonly IGameRepository _repository;

    public GetGamesQueryHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<GameDto>> Handle(
        GetGamesQuery request,
        CancellationToken cancellationToken)
    {
        var (games, totalCount) = await _repository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.Search,
            cancellationToken);

        return new PagedResult<GameDto>
        {
            Items = games
                .Select(game => new GameDto(
                    game.Id,
                    game.Title,
                    game.Slug,
                    game.ReleaseDate))
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}