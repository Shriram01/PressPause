using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Games.Commands.CreateGame;

public sealed class CreateGameCommandHandler
    : IRequestHandler<CreateGameCommand, Guid>
{
    private readonly IGameRepository _gameRepository;
    private readonly IPlatformRepository _platformRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IDeveloperRepository _developerRepository;

    public CreateGameCommandHandler(
        IGameRepository gameRepository,
        IPlatformRepository platformRepository,
        IGenreRepository genreRepository,
        IDeveloperRepository developerRepository)
    {
        _gameRepository = gameRepository;
        _platformRepository = platformRepository;
        _genreRepository = genreRepository;
        _developerRepository = developerRepository;
    }

    public async Task<Guid> Handle(
        CreateGameCommand request,
        CancellationToken cancellationToken)
    {
        bool slugExists = await _gameRepository.SlugExistsAsync(
            request.Slug,
            cancellationToken);

        if (slugExists)
        {
            throw new InvalidOperationException(
                "A game with this slug already exists.");
        }

        //Platform Validation
        foreach (var platformId in request.PlatformIds.Distinct())
        {
            bool platformExists = await _platformRepository.ExistsAsync(
                platformId,
                cancellationToken);

            if (!platformExists)
            {
                throw new KeyNotFoundException(
                    $"Platform '{platformId}' was not found.");
            }
        }

        //Genre Validation
        foreach (var genreId in request.GenreIds.Distinct())
        {
            bool genreExists = await _genreRepository.ExistsAsync(
                genreId,
                cancellationToken);

            if (!genreExists)
            {
                throw new KeyNotFoundException(
                    $"Genre '{genreId}' was not found.");
            }
        }

        //Developer Validation
        foreach (var developerId in request.DeveloperIds.Distinct())
        {
            bool developerExists = await _developerRepository.ExistsAsync(
                developerId,
                cancellationToken);

            if (!developerExists)
            {
                throw new KeyNotFoundException(
                    $"Developer '{developerId}' was not found.");
            }
        }

        var game = new Game
        {
            Title = request.Title,
            Slug = request.Slug,
            ReleaseDate = request.ReleaseDate
        };

        //Platform Injection
        foreach (var platformId in request.PlatformIds.Distinct())
        {
            game.GamePlatforms.Add(
                new GamePlatform(
                    game.Id,
                    platformId));
        }

        //Genre Injection
        foreach (var genreId in request.GenreIds.Distinct())
        {
            game.GameGenres.Add(
                new GameGenre(
                    game.Id,
                    genreId));
        }

        //Developer Injection
        foreach (var developerId in request.DeveloperIds.Distinct())
        {
            game.GameDevelopers.Add(
                new GameDeveloper(
                    game.Id,
                    developerId));
        }

        await _gameRepository.AddAsync(
            game,
            cancellationToken);

        await _gameRepository.SaveChangesAsync(
            cancellationToken);

        return game.Id;
    }
}