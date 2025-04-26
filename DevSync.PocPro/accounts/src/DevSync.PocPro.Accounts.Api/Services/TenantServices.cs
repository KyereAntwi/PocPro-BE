namespace DevSync.PocPro.Accounts.Api.Services;

public class TenantServices(TenantDatabaseSettings databaseSettings, IServiceScopeFactory serviceScopeFactory) 
    : ITenantServices
{
    public async Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default)
    {
        var query = $"CREATE DATABASE \"{database}\"";
        var masterConnectionString = databaseSettings.MasterConnectionString;

        await using var masterConnection = new NpgsqlConnection(masterConnectionString);
        await masterConnection.OpenAsync(cancellationToken);
        var command = masterConnection.CreateCommand();
        command.CommandText = query;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        using var scope = serviceScopeFactory.CreateScope();
        
        var context = new DbContextOptionsBuilder<TenantTemplateDbContext>().UseNpgsql(connectionString).Options;

        await using var dbContext = new TenantTemplateDbContext(context);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}