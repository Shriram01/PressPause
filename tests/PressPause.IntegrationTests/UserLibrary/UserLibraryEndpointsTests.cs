using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using PressPause.Domain.Entities;
using PressPause.Infrastructure.Identity;
using PressPause.IntegrationTests.Authorization;
using PressPause.IntegrationTests.Infrastructure;
using Xunit;

namespace PressPause.IntegrationTests.UserLibrary;

[Collection(PostgreSqlCollection.Name)]
public sealed class UserLibraryEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _database;

    public UserLibraryEndpointsTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task LibraryEndpoints_RejectUnauthenticatedRequests()
    {
        await _database.ResetDatabaseAsync();

        using var factory = CreateFactory(null);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/users/me/library");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task AddGame_User_AddsGameToLibrary()
    {
        await _database.ResetDatabaseAsync();
        await EnsureUserExistsAsync("library-user-1");

        using var adminFactory = CreateFactory("Admin", "library-admin-1");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", "library-user-1");
        using var userClient = userFactory.CreateClient();

        var response = await userClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Adding game to library failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");

        var libraryResponse = await userClient.GetAsync(
            "/api/users/me/library");

        var libraryBody = await libraryResponse.Content.ReadAsStringAsync();

        Assert.True(
            libraryResponse.StatusCode == HttpStatusCode.OK,
            $"Getting library failed. Status: {libraryResponse.StatusCode}. " +
            $"Response: {libraryBody}");

        Assert.Contains(gameId.ToString(), libraryBody);
    }

    [Fact]
    public async Task AddGame_DuplicateEntry_IsRejected()
    {
        await _database.ResetDatabaseAsync();
        await EnsureUserExistsAsync("library-user-2");

        using var adminFactory = CreateFactory("Admin", "library-admin-2");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", "library-user-2");
        using var userClient = userFactory.CreateClient();

        var firstResponse = await userClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var firstBody = await firstResponse.Content.ReadAsStringAsync();

        Assert.True(
            firstResponse.StatusCode == HttpStatusCode.Created,
            $"First library POST failed. Status: {firstResponse.StatusCode}. " +
            $"Response: {firstBody}");

        var duplicateResponse = await userClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var duplicateBody =
            await duplicateResponse.Content.ReadAsStringAsync();

        Assert.True(
            duplicateResponse.StatusCode == HttpStatusCode.Conflict ||
            duplicateResponse.StatusCode == HttpStatusCode.BadRequest,
            $"Duplicate library POST should return Conflict or BadRequest. " +
            $"Actual status: {duplicateResponse.StatusCode}. " +
            $"Response: {duplicateBody}");
    }

    [Fact]
    public async Task UpdateStatus_ExistingLibraryEntry_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();
        await EnsureUserExistsAsync("library-user-3");

        using var adminFactory = CreateFactory("Admin", "library-admin-3");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", "library-user-3");
        using var userClient = userFactory.CreateClient();

        var addResponse = await userClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var addBody = await addResponse.Content.ReadAsStringAsync();

        Assert.True(
            addResponse.StatusCode == HttpStatusCode.Created,
            $"Adding game before status update failed. " +
            $"Status: {addResponse.StatusCode}. Response: {addBody}");

        var response = await userClient.PutAsJsonAsync(
            $"/api/users/me/library/{gameId}/status",
            new { Status = GameStatus.Playing });

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent,
            $"Updating library status failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task RemoveGame_ExistingLibraryEntry_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();
        await EnsureUserExistsAsync("library-user-4");

        using var adminFactory = CreateFactory("Admin", "library-admin-4");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", "library-user-4");
        using var userClient = userFactory.CreateClient();

        var addResponse = await userClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var addBody = await addResponse.Content.ReadAsStringAsync();

        Assert.True(
            addResponse.StatusCode == HttpStatusCode.Created,
            $"Adding game before removal failed. " +
            $"Status: {addResponse.StatusCode}. Response: {addBody}");

        var response = await userClient.DeleteAsync(
            $"/api/users/me/library/{gameId}");

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.NoContent,
            $"Removing library entry failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");
    }

    [Fact]
    public async Task Users_CannotAccessEachOthersLibraryEntries()
    {
        await _database.ResetDatabaseAsync();

        await EnsureUserExistsAsync("library-owner");
        await EnsureUserExistsAsync("different-user");

        using var adminFactory = CreateFactory("Admin", "library-admin-5");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var ownerFactory = CreateFactory("User", "library-owner");
        using var ownerClient = ownerFactory.CreateClient();

        var addResponse = await ownerClient.PostAsJsonAsync(
            "/api/users/me/library",
            new { GameId = gameId });

        var addBody = await addResponse.Content.ReadAsStringAsync();

        Assert.True(
            addResponse.StatusCode == HttpStatusCode.Created,
            $"Adding game for library owner failed. " +
            $"Status: {addResponse.StatusCode}. Response: {addBody}");

        using var otherUserFactory = CreateFactory("User", "different-user");
        using var otherUserClient = otherUserFactory.CreateClient();

        var libraryResponse = await otherUserClient.GetAsync(
            "/api/users/me/library");

        var libraryBody = await libraryResponse.Content.ReadAsStringAsync();

        Assert.True(
            libraryResponse.StatusCode == HttpStatusCode.OK,
            $"Getting the other user's library failed. " +
            $"Status: {libraryResponse.StatusCode}. Response: {libraryBody}");

        Assert.DoesNotContain(gameId.ToString(), libraryBody);
    }

    private async Task EnsureUserExistsAsync(string userId)
    {
        using var factory = CreateFactory("User", userId);
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var existingUser = await userManager.FindByIdAsync(userId);

        if (existingUser is not null)
        {
            return;
        }

        var user = new ApplicationUser
        {
            Id = userId,
            UserName = userId,
            Email = $"{userId}@integration.test",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(
            user,
            "TestPassword123!");

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(
                $"Could not create integration test user '{userId}': {errors}");
        }
    }

    private WebApplicationFactory<Program> CreateFactory(
        string? role,
        string userId = "integration-test-user")
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
                                    _database.ConnectionString
                            });
                    });

                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<TestRoleProvider>();

                    services.AddSingleton(
                        new TestRoleProvider(role, userId));

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme =
                            TestAuthHandler.TestScheme;

                        options.DefaultChallengeScheme =
                            TestAuthHandler.TestScheme;

                        options.DefaultForbidScheme =
                            TestAuthHandler.TestScheme;
                    }).AddScheme<
                        AuthenticationSchemeOptions,
                        TestAuthHandler>(
                            TestAuthHandler.TestScheme,
                            _ => { });
                });
            });
    }

    private static async Task<Guid> CreateGameAsync(HttpClient adminClient)
    {
        var uniqueId = Guid.NewGuid().ToString("N");

        var payload = new
        {
            Title = $"Integration Test Game {uniqueId}",
            Slug = $"integration-test-game-{uniqueId}",
            Description = "Game created by an integration test."
        };

        var response = await adminClient.PostAsJsonAsync(
            "/api/games",
            payload);

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.IsSuccessStatusCode,
            $"Creating a test game failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.String &&
            Guid.TryParse(root.GetString(), out var stringGameId))
        {
            return stringGameId;
        }

        if (root.ValueKind == JsonValueKind.Object)
        {
            if (root.TryGetProperty("id", out var idProperty) &&
                Guid.TryParse(idProperty.ToString(), out var objectGameId))
            {
                return objectGameId;
            }

            if (root.TryGetProperty("game", out var gameProperty) &&
                gameProperty.ValueKind == JsonValueKind.Object &&
                gameProperty.TryGetProperty("id", out idProperty) &&
                Guid.TryParse(idProperty.ToString(), out objectGameId))
            {
                return objectGameId;
            }
        }

        throw new InvalidOperationException(
            "Could not extract the game ID from the API response. " +
            $"Response: {responseBody}");
    }
}