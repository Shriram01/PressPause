using MediatR;
using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Games.Commands.UpdateGame;

public sealed class UpdateGameCommandHandler
    : IRequestHandler<UpdateGameCommand>
{
    private readonly IGameRepository _repository;
    private readonly IPlatformRepository _platformRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IDeveloperRepository _developerRepository;
    private readonly IPublisherRepository _publisherRepository;

    public UpdateGameCommandHandler(
        IGameRepository repository,
        IPlatformRepository platformRepository,
        IGenreRepository genreRepository,
        IDeveloperRepository developerRepository,
        IPublisherRepository publisherRepository)
    {
        _repository = repository;
        _platformRepository = platformRepository;
        _genreRepository = genreRepository;
        _developerRepository = developerRepository;
        _publisherRepository = publisherRepository;
    }

    public async Task Handle(
        UpdateGameCommand request,
        CancellationToken cancellationToken)
    {
        var game = await _repository.GetForUpdateAsync(
            request.Id,
            cancellationToken);

        if (game is null)
        {
            throw new KeyNotFoundException("Game not found.");
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
        
        //Publisher Validation
        foreach (var publisherId in request.PublisherIds.Distinct())
        {
            bool publisherExists = await _publisherRepository.ExistsAsync(
                publisherId,
                cancellationToken);

            if (!publisherExists)
            {
                throw new KeyNotFoundException(
                    $"Publisher '{publisherId}' was not found.");
            }
        }

        game.Update(
            request.Title,
            request.Slug,
            request.ReleaseDate);

        //Platform Injection
        game.GamePlatforms.Clear();

        foreach (var platformId in request.PlatformIds.Distinct())
        {
            game.GamePlatforms.Add(
                new GamePlatform(
                    game.Id,
                    platformId));
        }

        //Genre Injection
        game.GameGenres.Clear();

       foreach (var genreId in request.GenreIds.Distinct())
        {
            game.GameGenres.Add(
                new GameGenre(
                    game.Id,
                    genreId));
        }

        //Developer Injection
        game.GameDevelopers.Clear();

        foreach (var developerId in request.DeveloperIds.Distinct())
        {
            game.GameDevelopers.Add(
                new GameDeveloper(
                    game.Id,
                    developerId));
        }
        
        //Publisher Injection
        game.GamePublishers.Clear();

        foreach (var publisherId in request.PublisherIds.Distinct())
        {
            game.GamePublishers.Add(
                new GamePublisher(
                    game.Id,
                    publisherId));
        }

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}