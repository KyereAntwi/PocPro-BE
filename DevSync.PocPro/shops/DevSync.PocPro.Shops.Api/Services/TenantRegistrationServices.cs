namespace DevSync.PocPro.Shops.Api.Services;

public interface ITenantRegistrationServices
{
    Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default);
    Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default);
}
    
public class TenantRegistrationServices : ITenantRegistrationServices
{
    public async Task GenerateTenantDatabase(string database, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}