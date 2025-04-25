using DevSync.PocPro.Domain.Tenants;

namespace DevSync.PocPro.Domain.Interfaces;

public interface ITenantServices
{
    Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task GenerateTenantDatabaseAsync(string databaseName, CancellationToken cancellationToken = default);
    Task ApplyMigrationAsync(string connectionString, CancellationToken cancellationToken = default);
    Task<Tenant> GetTenantByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Tenant> DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Tenant>> GetAllTenantsAsync(CancellationToken cancellationToken = default);
}