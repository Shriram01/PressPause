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

        const string userId = "review-create-user";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-create-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var response = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 8, Text = "A well-designed game." });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {response.StatusCode}. Response: {body}");

        var reviewId = GetReviewId(body);

        Assert.NotEqual(Guid.Empty, reviewId);
    }

    [Fact]
    public async Task CreateReview_DuplicateReview_ReturnsConflict()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-duplicate-user";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-duplicate-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var firstResponse = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 7, Text = "First review." });

        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var duplicateResponse = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 9, Text = "Duplicate review." });

        Assert.Equal(HttpStatusCode.Conflict, duplicateResponse.StatusCode);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public async Task CreateReview_InvalidRating_ReturnsBadRequest(int rating)
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-invalid-rating-user";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-invalid-rating-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var response = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = rating, Text = "Invalid rating." });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Rating {rating} should be rejected. " +
            $"Status: {response.StatusCode}. Response: {body}");
    }

    [Fact]
    public async Task CreateReview_TextExceedsMaximumLength_ReturnsBadRequest()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-long-text-user";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-long-text-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var response = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 8, Text = new string('A', 2001) });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Oversized review text should be rejected. " +
            $"Status: {response.StatusCode}. Response: {body}");
    }

    [Fact]
    public async Task CreateReview_UnauthenticatedUser_ReturnsUnauthorized()
    {
        await _database.ResetDatabaseAsync();

        using var adminFactory = CreateFactory("Admin", "review-anonymous-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var anonymousFactory = CreateFactory(null, "anonymous-review-user");
        using var anonymousClient = anonymousFactory.CreateClient();

        var response = await anonymousClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 8, Text = "Unauthenticated review." });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateReview_Owner_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-update-owner";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-update-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var createResponse = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 6, Text = "Original review." });

        var createBody = await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {createResponse.StatusCode}. " +
            $"Response: {createBody}");

        var reviewId = GetReviewId(createBody);

        var updateResponse = await userClient.PutAsJsonAsync(
            $"/api/games/{gameId}/reviews/{reviewId}",
            new { Rating = 9, Text = "Updated review." });

        var updateBody = await updateResponse.Content.ReadAsStringAsync();

        Assert.True(
            updateResponse.StatusCode == HttpStatusCode.NoContent,
            $"Updating review failed. Status: {updateResponse.StatusCode}. " +
            $"Response: {updateBody}");
    }

    [Fact]
    public async Task UpdateReview_NonOwner_IsRejected()
    {
        await _database.ResetDatabaseAsync();

        const string ownerId = "review-security-owner";
        const string otherUserId = "review-security-other";

        await EnsureUserExistsAsync(ownerId);
        await EnsureUserExistsAsync(otherUserId);

        using var adminFactory = CreateFactory("Admin", "review-security-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var ownerFactory = CreateFactory("User", ownerId);
        using var ownerClient = ownerFactory.CreateClient();

        var createResponse = await ownerClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 7, Text = "Owner's review." });

        var createBody = await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {createResponse.StatusCode}. " +
            $"Response: {createBody}");

        var reviewId = GetReviewId(createBody);

        using var otherFactory = CreateFactory("User", otherUserId);
        using var otherClient = otherFactory.CreateClient();

        var response = await otherClient.PutAsJsonAsync(
            $"/api/games/{gameId}/reviews/{reviewId}",
            new { Rating = 10, Text = "Unauthorized update." });

        var body = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Forbidden,
            $"Non-owner update should return 404 or 403. " +
            $"Status: {response.StatusCode}. Response: {body}");
    }

    [Fact]
    public async Task UpdateReview_NonexistentReview_ReturnsNotFound()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-update-missing-user";
        await EnsureUserExistsAsync(userId);

        using var factory = CreateFactory("User", userId);
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"/api/games/{Guid.NewGuid()}/reviews/{Guid.NewGuid()}",
            new { Rating = 8, Text = "Review does not exist." });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteReview_Owner_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-delete-owner";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-delete-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var userFactory = CreateFactory("User", userId);
        using var userClient = userFactory.CreateClient();

        var createResponse = await userClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 8, Text = "Review to delete." });

        var createBody = await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {createResponse.StatusCode}. " +
            $"Response: {createBody}");

        var reviewId = GetReviewId(createBody);

        var deleteResponse = await userClient.DeleteAsync(
            $"/api/games/{gameId}/reviews/{reviewId}");

        var deleteBody = await deleteResponse.Content.ReadAsStringAsync();

        Assert.True(
            deleteResponse.StatusCode == HttpStatusCode.NoContent,
            $"Deleting review failed. Status: {deleteResponse.StatusCode}. " +
            $"Response: {deleteBody}");
    }

    [Fact]
    public async Task DeleteReview_NonOwner_IsRejected()
    {
        await _database.ResetDatabaseAsync();

        const string ownerId = "review-delete-owner-security";
        const string otherUserId = "review-delete-other-security";

        await EnsureUserExistsAsync(ownerId);
        await EnsureUserExistsAsync(otherUserId);

        using var adminFactory = CreateFactory("Admin", "review-delete-security-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var ownerFactory = CreateFactory("User", ownerId);
        using var ownerClient = ownerFactory.CreateClient();

        var createResponse = await ownerClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 7, Text = "Owner's review." });

        var createBody = await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {createResponse.StatusCode}. " +
            $"Response: {createBody}");

        var reviewId = GetReviewId(createBody);

        using var otherFactory = CreateFactory("User", otherUserId);
        using var otherClient = otherFactory.CreateClient();

        var deleteResponse = await otherClient.DeleteAsync(
            $"/api/games/{gameId}/reviews/{reviewId}");

        Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

        var publicResponse = await otherClient.GetAsync(
            $"/api/games/{gameId}/reviews");

        var publicBody = await publicResponse.Content.ReadAsStringAsync();

        Assert.True(
            publicResponse.StatusCode == HttpStatusCode.OK,
            $"Retrieving public reviews failed. " +
            $"Status: {publicResponse.StatusCode}. Response: {publicBody}");

        Assert.Contains(reviewId.ToString(), publicBody);
    }

    [Fact]
    public async Task DeleteReview_NonexistentReview_ReturnsNotFound()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-delete-missing-user";
        await EnsureUserExistsAsync(userId);

        using var factory = CreateFactory("User", userId);
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync(
            $"/api/games/{Guid.NewGuid()}/reviews/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetReviews_AnonymousUser_ReturnsReviews()
    {
        await _database.ResetDatabaseAsync();

        const string userId = "review-public-owner";
        await EnsureUserExistsAsync(userId);

        using var adminFactory = CreateFactory("Admin", "review-public-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        using var ownerFactory = CreateFactory("User", userId);
        using var ownerClient = ownerFactory.CreateClient();

        var createResponse = await ownerClient.PostAsJsonAsync(
            $"/api/games/{gameId}/reviews",
            new { Rating = 8, Text = "Public review content." });

        var createBody = await createResponse.Content.ReadAsStringAsync();

        Assert.True(
            createResponse.StatusCode == HttpStatusCode.Created,
            $"Creating review failed. Status: {createResponse.StatusCode}. " +
            $"Response: {createBody}");

        using var anonymousFactory = CreateFactory(null, "anonymous-review-reader");
        using var anonymousClient = anonymousFactory.CreateClient();

        var response = await anonymousClient.GetAsync(
            $"/api/games/{gameId}/reviews");

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"Public review retrieval failed. Status: {response.StatusCode}. " +
            $"Response: {responseBody}");

        Assert.Contains("Public review content.", responseBody);
    }

    [Fact]
    public async Task GetReviews_Pagination_ReturnsRequestedPage()
    {
        await _database.ResetDatabaseAsync();

        using var adminFactory = CreateFactory("Admin", "review-pagination-admin");
        using var adminClient = adminFactory.CreateClient();

        var gameId = await CreateGameAsync(adminClient);

        for (var rating = 1; rating <= 3; rating++)
        {
            var reviewerId = $"review-pagination-user-{rating}";
            await EnsureUserExistsAsync(reviewerId);

            using var reviewerFactory = CreateFactory("User", reviewerId);
            using var reviewerClient = reviewerFactory.CreateClient();

            var createResponse = await reviewerClient.PostAsJsonAsync(
                $"/api/games/{gameId}/reviews",
                new
                {
                    Rating = rating,
                    Text = $"Review rating {rating}."
                });

            var createBody = await createResponse.Content.ReadAsStringAsync();

            Assert.True(
                createResponse.StatusCode == HttpStatusCode.Created,
                $"Creating review {rating} failed. " +
                $"Status: {createResponse.StatusCode}. Response: {createBody}");
        }

        using var anonymousFactory = CreateFactory(null, "review-pagination-reader");
        using var anonymousClient = anonymousFactory.CreateClient();

        var response = await anonymousClient.GetAsync(
            $"/api/games/{gameId}/reviews?page=2&pageSize=2");

        var responseBody = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == HttpStatusCode.OK,
            $"Retrieving paginated reviews failed. " +
            $"Status: {response.StatusCode}. Response: {responseBody}");

        using var document = JsonDocument.Parse(responseBody);
        var root = document.RootElement;

        Assert.Equal(2, root.GetProperty("page").GetInt32());
        Assert.Equal(2, root.GetProperty("pageSize").GetInt32());
        Assert.Equal(3, root.GetProperty("totalCount").GetInt32());
        Assert.Equal(1, root.GetProperty("items").GetArrayLength());
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

                builder.ConfigureAppConfiguration((_, configurationBuilder) =>
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

    private static Guid GetReviewId(string responseBody)
    {
        using var document = JsonDocument.Parse(responseBody);

        if (document.RootElement.TryGetProperty("id", out var idProperty) &&
            Guid.TryParse(idProperty.ToString(), out var reviewId))
        {
            return reviewId;
        }

        throw new InvalidOperationException(
            $"Could not extract the review ID from the API response: {responseBody}");
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