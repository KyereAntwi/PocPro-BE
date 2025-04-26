namespace DevSync.PocPro.Accounts.Api.Interfaces;

public interface ITenantServices
{
    Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default);
    Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default);
}