using DevSync.PocPro.Shared.Domain.Utils;
using Npgsql;

namespace DevSync.PocPro.Shops.Api.Services;

public interface ITenantRegistrationServices
{
    Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default);
    Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default);
}
    
public class TenantRegistrationServices(
    IServiceScopeFactory serviceScopeFactory,
    TenantDatabaseSettings databaseSettings) : ITenantRegistrationServices
{
    public async Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default)
    {
        var query = $"CREATE DATABASE \"{database}\"";
        var masterConnectionString = databaseSettings.MasterConnectionString;

        await using var masterConnection = new NpgsqlConnection(masterConnectionString);
        await masterConnection.OpenAsync();
        var command = masterConnection.CreateCommand();
        command.CommandText = query;
        await command.ExecuteNonQueryAsync();
    }

    public async Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        
        var context = new DbContextOptionsBuilder<MainShopTemplateDbContext>().UseNpgsql(connectionString).Options;

        await using var dbContext = new MainShopTemplateDbContext(context);
        await dbContext.Database.MigrateAsync();
    }
}