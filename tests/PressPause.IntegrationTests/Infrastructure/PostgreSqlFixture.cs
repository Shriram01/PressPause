using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PressPause.Infrastructure.Persistence;

namespace PressPause.IntegrationTests.Infrastructure;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private const string ConnectionStringEnvironmentVariable =
        "PRESSPAUSE_INTEGRATION_TESTS_CONNECTION_STRING";

    private const string DatabaseName = "PressPauseIntegrationTests";

    public PostgreSqlFixture()
    {
        ConnectionString = GetConnectionString();
    }

    public string ConnectionString { get; }

    public async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.EnsureDeletedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await using var context = CreateDbContext();
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    private static string GetConnectionString()
    {
        var connectionString = Environment.GetEnvironmentVariable(
            ConnectionStringEnvironmentVariable);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Set {ConnectionStringEnvironmentVariable} to a connection string for " +
                $"the {DatabaseName} database before running integration tests.");
        }

        var builder = new DbConnectionStringBuilder
        {
            ConnectionString = connectionString
        };

        if (!builder.TryGetValue("Database", out var database) ||
            !string.Equals(database?.ToString(), DatabaseName,
                StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Integration tests may only recreate the {DatabaseName} database.");
        }

        return connectionString;
    }
}
