using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PressPause.Application;
using PressPause.Application.Games.Commands.CreateGame;
using PressPause.Application.Games.Commands.UpdateGame;
using PressPause.Domain.Entities;
using PressPause.Infrastructure;
using PressPause.Infrastructure.Persistence;
using PressPause.IntegrationTests.Infrastructure;

namespace PressPause.IntegrationTests.Games;

[Collection(PostgreSqlCollection.Name)]
public sealed class GameRelationshipPersistenceTests
{
    private readonly PostgreSqlFixture _database;

    public GameRelationshipPersistenceTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task CreateGame_PersistsEachDistinctRelationship()
    {
        await _database.ResetDatabaseAsync();
        var related = await SeedRelationshipsAsync();

        using var services = CreateServices();
        using var scope = services.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CreateGameCommand, Guid>>();

        var gameId = await handler.Handle(
            new CreateGameCommand("Test Game", "test-game", new DateOnly(2026, 1, 1))
            {
                PlatformIds = [related.Platform.Id, related.Platform.Id],
                GenreIds = [related.Genre.Id],
                DeveloperIds = [related.Developer.Id],
                PublisherIds = [related.Publisher.Id],
                StoreIds = [related.Store.Id],
                TagIds = [related.Tag.Id, related.Tag.Id]
            },
            CancellationToken.None);

        await using var verificationContext = _database.CreateDbContext();
        var game = await LoadGameAsync(verificationContext, gameId);

        Assert.Equal("Test Game", game.Title);
        Assert.Single(game.GamePlatforms);
        Assert.Single(game.GameGenres);
        Assert.Single(game.GameDevelopers);
        Assert.Single(game.GamePublishers);
        Assert.Single(game.GameStores);
        Assert.Single(game.GameTags);
    }

    [Fact]
    public async Task UpdateGame_ReplacesExistingRelationships()
    {
        await _database.ResetDatabaseAsync();
        var initial = await SeedRelationshipsAsync();
        var replacement = await SeedRelationshipsAsync("replacement");

        using var services = CreateServices();
        using var scope = services.CreateScope();

        var createHandler = scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CreateGameCommand, Guid>>();
        var updateHandler = scope.ServiceProvider.GetRequiredService<
            IRequestHandler<UpdateGameCommand>>();

        var gameId = await createHandler.Handle(
            new CreateGameCommand("Test Game", "test-game", null)
            {
                PlatformIds = [initial.Platform.Id],
                GenreIds = [initial.Genre.Id],
                DeveloperIds = [initial.Developer.Id],
                PublisherIds = [initial.Publisher.Id],
                StoreIds = [initial.Store.Id],
                TagIds = [initial.Tag.Id]
            },
            CancellationToken.None);

        await updateHandler.Handle(
            new UpdateGameCommand(gameId, "Updated Game", "updated-game", null)
            {
                PlatformIds = [replacement.Platform.Id],
                GenreIds = [replacement.Genre.Id],
                DeveloperIds = [replacement.Developer.Id],
                PublisherIds = [replacement.Publisher.Id],
                StoreIds = [replacement.Store.Id],
                TagIds = [replacement.Tag.Id]
            },
            CancellationToken.None);

        await using var verificationContext = _database.CreateDbContext();
        var game = await LoadGameAsync(verificationContext, gameId);

        Assert.Equal("Updated Game", game.Title);
        Assert.Equal([replacement.Platform.Id], game.GamePlatforms.Select(x => x.PlatformId));
        Assert.Equal([replacement.Genre.Id], game.GameGenres.Select(x => x.GenreId));
        Assert.Equal([replacement.Developer.Id], game.GameDevelopers.Select(x => x.DeveloperId));
        Assert.Equal([replacement.Publisher.Id], game.GamePublishers.Select(x => x.PublisherId));
        Assert.Equal([replacement.Store.Id], game.GameStores.Select(x => x.StoreId));
        Assert.Equal([replacement.Tag.Id], game.GameTags.Select(x => x.TagId));
    }

    [Fact]
    public async Task CreateGame_MissingPlatform_ThrowsKeyNotFoundException()
    {
        await _database.ResetDatabaseAsync();

        using var services = CreateServices();
        using var scope = services.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CreateGameCommand, Guid>>();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(
            new CreateGameCommand("Test Game", "test-game", null)
            {
                PlatformIds = [Guid.NewGuid()]
            },
            CancellationToken.None));
    }

    [Fact]
    public async Task CreateGame_DuplicateSlug_ThrowsInvalidOperationException()
    {
        await _database.ResetDatabaseAsync();

        using var services = CreateServices();
        using var scope = services.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<
            IRequestHandler<CreateGameCommand, Guid>>();

        await handler.Handle(
            new CreateGameCommand("First Game", "duplicate-slug", null),
            CancellationToken.None);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
            new CreateGameCommand("Second Game", "duplicate-slug", null),
            CancellationToken.None));

        await using var verificationContext = _database.CreateDbContext();
        Assert.Equal(1, await verificationContext.Games.CountAsync());
    }

    private ServiceProvider CreateServices()
    {
        var configuration = new ConfigurationManager
        {
            ["ConnectionStrings:DefaultConnection"] = _database.ConnectionString
        };

        var services = new ServiceCollection();
        services.AddApplication();
        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }

    private async Task<RelatedEntities> SeedRelationshipsAsync(string suffix = "initial")
    {
        var related = new RelatedEntities(
            new Platform($"Platform {suffix}", $"platform-{suffix}"),
            new Genre($"Genre {suffix}", $"genre-{suffix}"),
            new Developer($"Developer {suffix}", $"developer-{suffix}"),
            new Publisher($"Publisher {suffix}", $"publisher-{suffix}"),
            new Store($"Store {suffix}", $"store-{suffix}"),
            new Tag($"Tag {suffix}", $"tag-{suffix}"));

        await using var context = _database.CreateDbContext();
        context.AddRange(
            related.Platform,
            related.Genre,
            related.Developer,
            related.Publisher,
            related.Store,
            related.Tag);
        await context.SaveChangesAsync();

        return related;
    }

    private static Task<Game> LoadGameAsync(
        ApplicationDbContext context,
        Guid gameId)
    {
        return context.Games
            .Include(x => x.GamePlatforms)
            .Include(x => x.GameGenres)
            .Include(x => x.GameDevelopers)
            .Include(x => x.GamePublishers)
            .Include(x => x.GameStores)
            .Include(x => x.GameTags)
            .SingleAsync(x => x.Id == gameId);
    }

    private sealed record RelatedEntities(
        Platform Platform,
        Genre Genre,
        Developer Developer,
        Publisher Publisher,
        Store Store,
        Tag Tag);
}
