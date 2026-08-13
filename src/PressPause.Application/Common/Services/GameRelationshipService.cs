using PressPause.Application.Common.Interfaces;
using PressPause.Domain.Entities;

namespace PressPause.Application.Common.Services;

public sealed class GameRelationshipService : IGameRelationshipService
{
    private readonly IPlatformRepository _platformRepository;
    private readonly IGenreRepository _genreRepository;
    private readonly IDeveloperRepository _developerRepository;
    private readonly IPublisherRepository _publisherRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ITagRepository _tagRepository;

    public GameRelationshipService(
        IPlatformRepository platformRepository,
        IGenreRepository genreRepository,
        IDeveloperRepository developerRepository,
        IPublisherRepository publisherRepository,
        IStoreRepository storeRepository,
        ITagRepository tagRepository)
    {
        _platformRepository = platformRepository;
        _genreRepository = genreRepository;
        _developerRepository = developerRepository;
        _publisherRepository = publisherRepository;
        _storeRepository = storeRepository;
        _tagRepository = tagRepository;
    }

    public async Task ValidateAsync(
        IReadOnlyCollection<Guid> platformIds,
        IReadOnlyCollection<Guid> genreIds,
        IReadOnlyCollection<Guid> developerIds,
        IReadOnlyCollection<Guid> publisherIds,
        IReadOnlyCollection<Guid> storeIds,
        IReadOnlyCollection<Guid> tagIds,
        CancellationToken cancellationToken = default)
    {
        await ValidatePlatformsAsync(platformIds, cancellationToken);
        await ValidateGenresAsync(genreIds, cancellationToken);
        await ValidateDevelopersAsync(developerIds, cancellationToken);
        await ValidatePublishersAsync(publisherIds, cancellationToken);
        await ValidateStoresAsync(storeIds, cancellationToken);
        await ValidateTagsAsync(tagIds, cancellationToken);
    }

    public void SetRelationships(
        Game game,
        IReadOnlyCollection<Guid> platformIds,
        IReadOnlyCollection<Guid> genreIds,
        IReadOnlyCollection<Guid> developerIds,
        IReadOnlyCollection<Guid> publisherIds,
        IReadOnlyCollection<Guid> storeIds,
        IReadOnlyCollection<Guid> tagIds)
    {
        game.GamePlatforms.Clear();
        game.GameGenres.Clear();
        game.GameDevelopers.Clear();
        game.GamePublishers.Clear();
        game.GameStores.Clear();
        game.GameTags.Clear();

        foreach (var platformId in platformIds.Distinct())
        {
            game.GamePlatforms.Add(
                new GamePlatform(game.Id, platformId));
        }

        foreach (var genreId in genreIds.Distinct())
        {
            game.GameGenres.Add(
                new GameGenre(game.Id, genreId));
        }

        foreach (var developerId in developerIds.Distinct())
        {
            game.GameDevelopers.Add(
                new GameDeveloper(game.Id, developerId));
        }

        foreach (var publisherId in publisherIds.Distinct())
        {
            game.GamePublishers.Add(
                new GamePublisher(game.Id, publisherId));
        }

        foreach (var storeId in storeIds.Distinct())
        {
            game.GameStores.Add(
                new GameStore(game.Id, storeId));
        }

        foreach (var tagId in tagIds.Distinct())
        {
            game.GameTags.Add(
                new GameTag(game.Id, tagId));
        }
    }

    private async Task ValidatePlatformsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _platformRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Platform '{id}' was not found.");
            }
        }
    }

    private async Task ValidateGenresAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _genreRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Genre '{id}' was not found.");
            }
        }
    }

    private async Task ValidateDevelopersAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _developerRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Developer '{id}' was not found.");
            }
        }
    }

    private async Task ValidatePublishersAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _publisherRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Publisher '{id}' was not found.");
            }
        }
    }

    private async Task ValidateStoresAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _storeRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Store '{id}' was not found.");
            }
        }
    }

    private async Task ValidateTagsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        foreach (var id in ids.Distinct())
        {
            if (!await _tagRepository.ExistsAsync(id, cancellationToken))
            {
                throw new KeyNotFoundException(
                    $"Tag '{id}' was not found.");
            }
        }
    }
}