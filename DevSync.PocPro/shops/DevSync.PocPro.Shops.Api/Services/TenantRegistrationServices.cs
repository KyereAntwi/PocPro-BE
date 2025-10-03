using Npgsql;

namespace DevSync.PocPro.Shops.Api.Services;

public interface ITenantRegistrationServices
{
    Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default);
    Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default);
    Task<string?> DeployTenantFrontendAsync(string tenantId, CancellationToken cancellationToken = default);
}
    
public class TenantRegistrationServices(
    IServiceScopeFactory serviceScopeFactory,
    TenantDatabaseSettings databaseSettings,
    IHttpClientFactory httpClientFactory,
    ILogger<TenantRegistrationServices> logger) : ITenantRegistrationServices
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
        
        var context = new DbContextOptionsBuilder<MainShopTemplateDbContext>().UseNpgsql(connectionString).Options;

        await using var dbContext = new MainShopTemplateDbContext(context);
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public async Task<string?> DeployTenantFrontendAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        var client = httpClientFactory.CreateClient("vercel");
        
        var payload = new
        {
            name = tenantId + "_" + "pocpro",
            env = new Dictionary<string, string>
            {
                { "TENANT_IDENTIFIER", tenantId }
            },
            gitSource = new
            {
                type = "github",
                repoId = "your_repo_id_here",
                ref_ = "main"
            },
            buildCommand = "npm run build",
            outputDirectory = "build",
            framework = "create-react-app"
        };
        
        var response = await client.PostAsJsonAsync("/v13/deployments", payload, cancellationToken);

        if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync(cancellationToken);
        
        logger.LogError("Failed to deploy tenant frontend. Status Code: {StatusCode}, Response: {Response}",
            response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
            
        return null;
    }
}