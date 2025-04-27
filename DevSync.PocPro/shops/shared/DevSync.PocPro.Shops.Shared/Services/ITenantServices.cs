namespace DevSync.PocPro.Shops.Shared.Services;

public interface ITenantServices
{
    Task<Tenant?> GetTenantByUserIdAsync(string userId);
}

public record Tenant(string ConnectionString);