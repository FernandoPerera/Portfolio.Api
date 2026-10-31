using Microsoft.EntityFrameworkCore;
using Npgsql;
using Portfolio.Api.Shared.Infrastructure.Data;
using Testcontainers.PostgreSql;

namespace Portfolio.Api.Tests.Shared.Infrastructure;

/// <summary>
/// One PostgreSQL container shared by the whole test run. The schema comes from the SQL migration scripts
/// and is loaded once into a template database; every test gets its own copy so tests can run in parallel.
/// The container is removed by Testcontainers when the test process ends.
/// </summary>
public static class PostgresTestDatabase
{
    private const string TemplateName = "portfolio_template";
    private static readonly Lock CreationLock = new();
    private static readonly Lazy<PostgreSqlContainer> Container = new(StartContainer);

    public static string CreateConnectionString()
    {
        lock (CreationLock)
        {
            var admin = Container.Value.GetConnectionString();
            var name = $"portfolio_{Guid.NewGuid():N}";
            Execute(admin, $"CREATE DATABASE \"{name}\" TEMPLATE \"{TemplateName}\"");

            return new NpgsqlConnectionStringBuilder(admin) { Database = name }.ConnectionString;
        }
    }

    public static DbContextOptions<AppDbContext> CreateOptions() => CreateOptions(CreateConnectionString());

    public static AppDbContext CreateContext() => new(CreateOptions());

    public static AppDbContext CreateUnreachableContext() =>
        new(CreateOptions("Host=localhost;Port=1;Database=unreachable;Username=none;Password=none;Timeout=1"));

    private static DbContextOptions<AppDbContext> CreateOptions(string connectionString) =>
        new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(connectionString).Options;

    private static PostgreSqlContainer StartContainer()
    {
        var container = new PostgreSqlBuilder("postgres:17-alpine").Build();
        container.StartAsync().GetAwaiter().GetResult();

        var admin = container.GetConnectionString();
        Execute(admin, $"CREATE DATABASE \"{TemplateName}\"");
        var template = new NpgsqlConnectionStringBuilder(admin) { Database = TemplateName }.ConnectionString;
        foreach (var script in ReadMigrationScripts())
        {
            Execute(template, script);
        }

        return container;
    }

    private static IEnumerable<string> ReadMigrationScripts() =>
        Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "database", "migrations"), "*.sql")
            .Order()
            .Select(File.ReadAllText);

    // Pooling is off so no idle connection keeps the template database busy while it is being copied.
    private static void Execute(string connectionString, string sql)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString) { Pooling = false };
        using var connection = new NpgsqlConnection(builder.ConnectionString);
        connection.Open();
        using var command = new NpgsqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
}
