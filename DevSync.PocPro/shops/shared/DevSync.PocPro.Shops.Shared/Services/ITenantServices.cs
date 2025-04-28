namespace DevSync.PocPro.Shops.Shared.Services;

public interface ITenantServices
{
    Task<Tenant?> GetTenantByUserIdAsync(string userId);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
}

public record Tenant(string ConnectionString, string UniqueIdentifier, string SubscriptionType);