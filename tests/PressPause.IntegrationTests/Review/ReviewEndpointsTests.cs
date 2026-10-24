
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
using PressPause.Infrastructure.Identity;
using PressPause.IntegrationTests.Authorization;
using PressPause.IntegrationTests.Infrastructure;
using Xunit;

namespace PressPause.IntegrationTests.Reviews;

[Collection(PostgreSqlCollection.Name)]
public sealed class ReviewEndpointsTests : IAsyncLifetime
{
    private readonly PostgreSqlFixture _database;

    public ReviewEndpointsTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateReview_AuthenticatedUser_ReturnsCreated()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-user-1";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-admin-1");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var response = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new
            {
                Rating = 8,
                Text = "A well-designed game."
            });

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");

        using var document = JsonDocument.Parse(responseBody);

        Assert.True(
            document.RootElement.TryGetProperty("id", out var idProperty),
            $"Response did not contain a review ID. Response: {responseBody}");

        Assert.True(
            Guid.TryParse(idProperty.GetString(), out var reviewId),
            $"Response contained an invalid review ID. Response: {responseBody}");

        Assert.NotEqual(Guid.Empty, reviewId);
    }

    private async Task EnsureUserExistsAsync(string userId)
    {
        using var factory = CreateFactory("User", userId);
        using var scope = factory.Services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var existingUser = await userManager.FindByIdAsync(userId);

        if (existingUser is not null)
            return;

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
