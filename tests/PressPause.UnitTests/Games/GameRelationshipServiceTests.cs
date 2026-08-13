using Moq;
using PressPause.Application.Common.Interfaces;
using PressPause.Application.Common.Services;
using PressPause.Domain.Entities;

namespace PressPause.UnitTests.Games;

public sealed class GameRelationshipServiceTests
{
    [Fact]
    public void SetRelationships_DuplicateIds_AddsEachRelationshipOnce()
    {
        var service = new GameRelationshipService(
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);

        var game = new Game
        {
            Title = "Test Game",
            Slug = "test-game"
        };

        var platformId = Guid.NewGuid();
        var genreId = Guid.NewGuid();
        var developerId = Guid.NewGuid();
        var publisherId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var tagId = Guid.NewGuid();

        service.SetRelationships(
            game,
            [platformId, platformId],
            [genreId, genreId],
            [developerId, developerId],
            [publisherId, publisherId],
            [storeId, storeId],
            [tagId, tagId]);

        Assert.Single(game.GamePlatforms);
        Assert.Single(game.GameGenres);
        Assert.Single(game.GameDevelopers);
        Assert.Single(game.GamePublishers);
        Assert.Single(game.GameStores);
        Assert.Single(game.GameTags);
    }

    [Fact]
    public async Task ValidateAsync_InvalidPlatform_ThrowsKeyNotFoundException()
    {
        var platformRepository = new Mock<IPlatformRepository>();

        platformRepository
            .Setup(x => x.ExistsAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = new GameRelationshipService(
            platformRepository.Object,
            null!,
            null!,
            null!,
            null!,
            null!);

        var invalidPlatformId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            service.ValidateAsync(
                [invalidPlatformId],
                [],
                [],
                [],
                [],
                []));
    }
    [Fact]
public async Task ValidateAsync_InvalidGenre_ThrowsKeyNotFoundException()
{
    var genreRepository = new Mock<IGenreRepository>();

    genreRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var service = new GameRelationshipService(
        null!,
        genreRepository.Object,
        null!,
        null!,
        null!,
        null!);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        service.ValidateAsync(
            [],
            [Guid.NewGuid()],
            [],
            [],
            [],
            []));
}

[Fact]
public async Task ValidateAsync_InvalidDeveloper_ThrowsKeyNotFoundException()
{
    var repository = new Mock<IDeveloperRepository>();

    repository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var service = new GameRelationshipService(
        null!,
        null!,
        repository.Object,
        null!,
        null!,
        null!);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        service.ValidateAsync(
            [],
            [],
            [Guid.NewGuid()],
            [],
            [],
            []));
}

[Fact]
public async Task ValidateAsync_InvalidPublisher_ThrowsKeyNotFoundException()
{
    var repository = new Mock<IPublisherRepository>();

    repository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var service = new GameRelationshipService(
        null!,
        null!,
        null!,
        repository.Object,
        null!,
        null!);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        service.ValidateAsync(
            [],
            [],
            [],
            [Guid.NewGuid()],
            [],
            []));
}

[Fact]
public async Task ValidateAsync_InvalidStore_ThrowsKeyNotFoundException()
{
    var repository = new Mock<IStoreRepository>();

    repository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var service = new GameRelationshipService(
        null!,
        null!,
        null!,
        null!,
        repository.Object,
        null!);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        service.ValidateAsync(
            [],
            [],
            [],
            [],
            [Guid.NewGuid()],
            []));
}

[Fact]
public async Task ValidateAsync_InvalidTag_ThrowsKeyNotFoundException()
{
    var repository = new Mock<ITagRepository>();

    repository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(false);

    var service = new GameRelationshipService(
        null!,
        null!,
        null!,
        null!,
        null!,
        repository.Object);

    await Assert.ThrowsAsync<KeyNotFoundException>(() =>
        service.ValidateAsync(
            [],
            [],
            [],
            [],
            [],
            [Guid.NewGuid()]));
}
[Fact]
public async Task ValidateAsync_AllRelationshipsExist_CompletesSuccessfully()
{
    var platformRepository = new Mock<IPlatformRepository>();
    var genreRepository = new Mock<IGenreRepository>();
    var developerRepository = new Mock<IDeveloperRepository>();
    var publisherRepository = new Mock<IPublisherRepository>();
    var storeRepository = new Mock<IStoreRepository>();
    var tagRepository = new Mock<ITagRepository>();

    platformRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    genreRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    developerRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    publisherRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    storeRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    tagRepository
        .Setup(x => x.ExistsAsync(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()))
        .ReturnsAsync(true);

    var service = new GameRelationshipService(
        platformRepository.Object,
        genreRepository.Object,
        developerRepository.Object,
        publisherRepository.Object,
        storeRepository.Object,
        tagRepository.Object);

    await service.ValidateAsync(
        [Guid.NewGuid()],
        [Guid.NewGuid()],
        [Guid.NewGuid()],
        [Guid.NewGuid()],
        [Guid.NewGuid()],
        [Guid.NewGuid()]);
}
}