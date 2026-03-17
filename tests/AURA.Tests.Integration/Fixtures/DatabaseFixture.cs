using System.Data.Common;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace AURA.Tests.Integration.Fixtures;

public sealed class DatabaseFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    private Respawner? _respawner;

    public string ConnectionString => _container?.GetConnectionString()
        ?? throw new InvalidOperationException("PostgreSQL container has not been initialized.");

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:16-alpine")
                .WithDatabase("aura_integration")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();

            await _container.StartAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Docker/Testcontainers is unavailable. Start Docker Desktop to run integration tests.",
                ex);
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
        }
    }

    public async Task InitializeRespawnerAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
            TablesToIgnore =
            [
                new Table("public", "__EFMigrationsHistory"),
                new Table("public", "AspNetRoles"),
                new Table("public", "AspNetUsers"),
                new Table("public", "AspNetUserRoles"),
                new Table("public", "AspNetRoleClaims"),
                new Table("public", "AspNetUserClaims"),
                new Table("public", "AspNetUserLogins"),
                new Table("public", "AspNetUserTokens"),
                new Table("public", "Organisations"),
                new Table("public", "Ophthalmologists"),
                new Table("public", "Patients"),
                new Table("public", "Permissions"),
                new Table("public", "RolePermissions"),
                new Table("public", "ContractTemplates"),
                new Table("public", "ContractTemplateVariables")
            ]
        });
    }

    public async Task ResetAsync()
    {
        if (_respawner is null)
        {
            throw new InvalidOperationException("Respawner is not initialized.");
        }

        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }
}
