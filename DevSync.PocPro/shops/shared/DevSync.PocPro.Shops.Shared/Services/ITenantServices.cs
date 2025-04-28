using DevSync.PocPro.Shared.Domain.Enums;

namespace DevSync.PocPro.Shops.Shared.Services;

public interface ITenantServices
{
    Task<Tenant?> GetTenantByUserIdAsync(string userId);
    Task<IEnumerable<Tenant>> GetAllTenantsAsync();
    Task<bool> UserHasRequiredPermissionAsync(PermissionType permissionType, string userId);
}

public record Tenant(string ConnectionString, string UniqueIdentifier, string SubscriptionType);