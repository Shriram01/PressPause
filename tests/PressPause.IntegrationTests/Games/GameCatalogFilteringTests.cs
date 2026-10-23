using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using PressPause.Domain.Entities;
using PressPause.Infrastructure.Persistence;
using PressPause.IntegrationTests.Infrastructure;
using Xunit;

namespace PressPause.IntegrationTests.Games;

[Collection(PostgreSqlCollection.Name)]
public sealed class GameCatalogFilteringTests
{
    private readonly PostgreSqlFixture _database;

    public GameCatalogFilteringTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task GetGames_WithGenreId_ReturnsOnlyMatchingGames()
    {
        await _database.ResetDatabaseAsync();
        var data = await SeedCatalogAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PagedGamesResponse>(
            $"/api/games?genreId={data.GenreA.Id}");

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, game =>
            Assert.Contains(game.Genres, genre => genre.Id == data.GenreA.Id));
    }

    [Fact]
    public async Task GetGames_WithPlatformId_ReturnsOnlyMatchingGames()
    {
        await _database.ResetDatabaseAsync();
        var data = await SeedCatalogAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PagedGamesResponse>(
            $"/api/games?platformId={data.PlatformA.Id}");

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, game =>
            Assert.Contains(game.Platforms, platform => platform.Id == data.PlatformA.Id));
    }

    [Fact]
    public async Task GetGames_WithBothFilters_ReturnsOnlyGamesMatchingBoth()
    {
        await _database.ResetDatabaseAsync();
        var data = await SeedCatalogAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PagedGamesResponse>(
            $"/api/games?genreId={data.GenreA.Id}&platformId={data.PlatformA.Id}");

        Assert.NotNull(result);
        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Items);
        Assert.Equal("Game One", result.Items[0].Title);
    }

    [Fact]
    public async Task GetGames_WithUnknownGenreId_ReturnsNoGames()
    {
        await _database.ResetDatabaseAsync();
        await SeedCatalogAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PagedGamesResponse>(
            $"/api/games?genreId={Guid.NewGuid()}");

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetGames_WithGenreFilterAndPagination_ReturnsFilteredTotalCount()
    {
        await _database.ResetDatabaseAsync();
        var data = await SeedCatalogAsync();

        using var factory = CreateFactory();
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PagedGamesResponse>(
            $"/api/games?genreId={data.GenreA.Id}&page=1&pageSize=1");

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(1, result.Page);
        Assert.Equal(1, result.PageSize);
    }

    private async Task<CatalogSeed> SeedCatalogAsync()
    {
        await using var context = _database.CreateDbContext();

        var genreA = new Genre("Genre A", "genre-a");
        var genreB = new Genre("Genre B", "genre-b");
        var platformA = new Platform("Platform A", "platform-a");
        var platformB = new Platform("Platform B", "platform-b");

        context.AddRange(genreA, genreB, platformA, platformB);
        await context.SaveChangesAsync();

        var gameOne = CreateGame("Game One", "game-one");
        var gameTwo = CreateGame("Game Two", "game-two");
        var gameThree = CreateGame("Game Three", "game-three");

        context.Games.AddRange(gameOne, gameTwo, gameThree);
        await context.SaveChangesAsync();

        context.AddRange(
            new GameGenre(gameOne.Id, genreA.Id),
            new GameGenre(gameTwo.Id, genreA.Id),
            new GameGenre(gameThree.Id, genreB.Id),
            new GamePlatform(gameOne.Id, platformA.Id),
            new GamePlatform(gameTwo.Id, platformB.Id),
            new GamePlatform(gameThree.Id, platformA.Id));

        await context.SaveChangesAsync();

        return new CatalogSeed(genreA, platformA);
    }

    private static Game CreateGame(string title, string slug)
    {
        return new Game
        {
            Title = title,
            Slug = slug
        };
    }

    private WebApplicationFactory<Program> CreateFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureAppConfiguration(
                    (_, configurationBuilder) =>
                    {
                        configurationBuilder.AddInMemoryCollection(
                            new Dictionary<string, string?>
                            {
                                ["ConnectionStrings:DefaultConnection"] =
                                    _database.ConnectionString,
                                ["Jwt:Issuer"] = "PressPause.IntegrationTests",
                                ["Jwt:Audience"] = "PressPause.IntegrationTests",
                                ["Jwt:SecretKey"] =
                                    "integration-test-secret-key-at-least-32-characters-long",
                                ["Jwt:ExpiryMinutes"] = "60"
                            });
                    });
            });
    }

    private sealed record CatalogSeed(Genre GenreA, Platform PlatformA);

    private sealed record PagedGamesResponse(
        List<GameResponse> Items,
        int Page,
        int PageSize,
        int TotalCount);

    private sealed record GameResponse(
        Guid Id,
        string Title,
        string Slug,
        DateOnly? ReleaseDate,
        List<PlatformResponse> Platforms,
        List<GenreResponse> Genres);

    private sealed record PlatformResponse(Guid Id, string Name, string Slug);

    private sealed record GenreResponse(Guid Id, string Name, string Slug);
}