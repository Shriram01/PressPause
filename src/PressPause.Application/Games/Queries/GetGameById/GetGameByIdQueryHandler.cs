using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Games.Queries.GetGames;

namespace PressPause.Application.Games.Queries.GetGameById;

public sealed class GetGameByIdQueryHandler
    : IRequestHandler<GetGameByIdQuery, GameDetailsDto?>
{
    private readonly IGameRepository _repository;

    public GetGameByIdQueryHandler(IGameRepository repository)
    {
        _repository = repository;
    }

    public async Task<GameDetailsDto?> Handle(
        GetGameByIdQuery request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (game is null)
        {
            return null;
        }

        return new GameDetailsDto(
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
                .ToList()
        };
    }
}