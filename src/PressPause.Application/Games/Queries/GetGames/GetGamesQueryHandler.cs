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
            request.SortBy,
            request.Descending,
            cancellationToken);

        return new PagedResult<GameDto>
        {
            Items = games
                .Select(game => new GameDto(
                    game.Id,
                    game.Title,
                    game.Slug,
                    game.ReleaseDate)
                {
                    Platforms = game.GamePlatforms
                        .Select(x => new PlatformDto(
                            x.Platform.Id,
                            x.Platform.Name,
                            x.Platform.Slug))
                        .ToList(),

                    Genres = game.GameGenres
                        .Select(x => new GenreDto(
                            x.Genre.Id,
                            x.Genre.Name,
                            x.Genre.Slug))
                        .ToList(),

                    Developers = game.GameDevelopers
                        .Select(x => new DeveloperDto(
                            x.Developer.Id,
                            x.Developer.Name,
                            x.Developer.Slug))
                        .ToList(),

                    Publishers = game.GamePublishers
                        .Select(x => new PublisherDto(
                            x.Publisher.Id,
                            x.Publisher.Name,
                            x.Publisher.Slug))
                        .ToList(),
                    
                    Stores = game.GameStores
                        .Select(x => new StoreDto(
                            x.Store.Id,
                            x.Store.Name,
                            x.Store.Slug))
                        .ToList(),
                
                    Tags = game.GameTags
                        .Select(x => new TagDto(
                            x.Tag.Id,
                            x.Tag.Name,
                            x.Tag.Slug))
                        .ToList()
                })
                .ToList(),

            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
