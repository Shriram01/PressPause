using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PressPause.IntegrationTests.Infrastructure;
using Xunit;

namespace PressPause.IntegrationTests.Authorization;

[Collection(PostgreSqlCollection.Name)]
public sealed class GameAuthorizationTests
{
    private readonly PostgreSqlFixture _database;

    public GameAuthorizationTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task UpdateGame_RejectsNonAdmins()
    {
        using var unauthenticatedFactory = CreateFactory(null);
        using var unauthenticatedClient = unauthenticatedFactory.CreateClient();

        var unauthenticatedResponse = await unauthenticatedClient.PutAsJsonAsync(
            $"/api/games/{Guid.NewGuid()}",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedResponse.StatusCode);

        using var userFactory = CreateFactory("User");
        using var userClient = userFactory.CreateClient();

        var userResponse = await userClient.PutAsJsonAsync(
            $"/api/games/{Guid.NewGuid()}",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteGame_RejectsNonAdmins()
    {
        using var unauthenticatedFactory = CreateFactory(null);
        using var unauthenticatedClient = unauthenticatedFactory.CreateClient();

        var unauthenticatedResponse = await unauthenticatedClient.DeleteAsync(
            $"/api/games/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedResponse.StatusCode);

        using var userFactory = CreateFactory("User");
        using var userClient = userFactory.CreateClient();

        var userResponse = await userClient.DeleteAsync(
            $"/api/games/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }

    [Fact]
    public async Task CreateGame_RejectsNonAdmins()
    {
        using var unauthenticatedFactory = CreateFactory(null);
        using var unauthenticatedClient = unauthenticatedFactory.CreateClient();

        var unauthenticatedResponse = await unauthenticatedClient.PostAsJsonAsync(
            "/api/games",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Unauthorized, unauthenticatedResponse.StatusCode);

        using var userFactory = CreateFactory("User");
        using var userClient = userFactory.CreateClient();

        var userResponse = await userClient.PostAsJsonAsync(
            "/api/games",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }

    [Fact]
    public async Task CreateGame_Admin_ReturnsCreated()
    {
        await _database.ResetDatabaseAsync();

        using var factory = CreateFactory("Admin");
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGame_Admin_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();

        using var factory = CreateFactory("Admin");
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/games",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var gameId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var updatePayload = CreateGamePayload();
        updatePayload["title"] = "Updated Integration Test Game";
        updatePayload["slug"] = $"updated-integration-test-{Guid.NewGuid():N}";

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/games/{gameId}",
            new
            {
                id = gameId,
                title = updatePayload["title"],
                slug = updatePayload["slug"],
                releaseDate = (DateOnly?)null
            });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteGame_Admin_ReturnsNoContent()
    {
        await _database.ResetDatabaseAsync();

        using var factory = CreateFactory("Admin");
        using var client = factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync(
            "/api/games",
            CreateGamePayload());

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var gameId = await createResponse.Content.ReadFromJsonAsync<Guid>();

        var deleteResponse = await client.DeleteAsync($"/api/games/{gameId}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    private WebApplicationFactory<Program> CreateFactory(string? role)
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
                    services.AddSingleton(new TestRoleProvider(role));

                    services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme =
                                TestAuthHandler.TestScheme;

                            options.DefaultChallengeScheme =
                                TestAuthHandler.TestScheme;

                            options.DefaultForbidScheme =
                                TestAuthHandler.TestScheme;
                        })
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            TestAuthHandler.TestScheme,
                            _ => { });
                });
            });
    }

    private static Dictionary<string, object?> CreateGamePayload()
    {
        return new Dictionary<string, object?>
        {
            ["title"] = "Integration Test Game",
            ["slug"] = $"integration-test-{Guid.NewGuid():N}",
            ["releaseDate"] = (DateOnly?)null
        };
    }
}

internal sealed record TestRoleProvider(
    string? Role,
    string UserId = "integration-test-user");

internal sealed class TestAuthHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string TestScheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var roleProvider =
            Context.RequestServices.GetRequiredService<TestRoleProvider>();

        if (roleProvider.Role is null)
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
            new Claim(ClaimTypes.Name, "integration-test-user"),
            new Claim(ClaimTypes.Role, roleProvider.Role)
        };

        var identity = new ClaimsIdentity(claims, TestScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, TestScheme);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}