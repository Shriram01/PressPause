using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Xunit;

namespace PressPause.IntegrationTests.Authorization;

public sealed class GameAuthorizationTests
{
    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("User", HttpStatusCode.Forbidden)]
    public async Task UpdateGame_RejectsNonAdmins(
        string? role,
        HttpStatusCode expectedStatus)
    {
        using var factory = CreateFactory(role);
        using var client = factory.CreateClient();

        var id = Guid.NewGuid();

        var response = await client.PutAsJsonAsync(
            $"/api/games/{id}",
            new
            {
                id,
                title = "Updated Game",
                slug = "updated-game",
                releaseDate = (DateOnly?)null
            });

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("User", HttpStatusCode.Forbidden)]
    public async Task DeleteGame_RejectsNonAdmins(
        string? role,
        HttpStatusCode expectedStatus)
    {
        using var factory = CreateFactory(role);
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync(
            $"/api/games/{Guid.NewGuid()}");

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("User", HttpStatusCode.Forbidden)]
    public async Task CreateGame_RejectsNonAdmins(
        string? role,
        HttpStatusCode expectedStatus)
    {
        using var factory = CreateFactory(role);
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/games",
            new
            {
                title = "Authorization Test Game",
                slug = "authorization-test-game",
                releaseDate = (DateOnly?)null
            });

        Assert.Equal(expectedStatus, response.StatusCode);
    }

    private static WebApplicationFactory<Program> CreateFactory(
        string? role)
    {
        var connectionString =
            Environment.GetEnvironmentVariable(
                "PRESSPAUSE_INTEGRATION_TESTS_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set PRESSPAUSE_INTEGRATION_TESTS_CONNECTION_STRING " +
                "before running integration tests.");
        }

        var connectionBuilder =
            new System.Data.Common.DbConnectionStringBuilder
            {
                ConnectionString = connectionString
            };

        if (!connectionBuilder.TryGetValue("Database", out var database) ||
            !string.Equals(
                database?.ToString(),
                "PressPauseIntegrationTests",
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Authorization tests may only use PressPauseIntegrationTests.");
        }

        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureAppConfiguration((_, configuration) =>
                {
                    configuration.AddInMemoryCollection(
                        new Dictionary<string, string?>
                        {
                            ["ConnectionStrings:DefaultConnection"] =
                                connectionString
                        });
                });

                builder.ConfigureTestServices(services =>
                {
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
                        .AddScheme<
                            AuthenticationSchemeOptions,
                            TestAuthHandler>(
                            TestAuthHandler.TestScheme,
                            _ => { });

                    services.AddSingleton(new TestRoleProvider(role));
                });
            });
    }
}

internal sealed record TestRoleProvider(string? Role);

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
        var roleProvider = Context.RequestServices
            .GetRequiredService<TestRoleProvider>();

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