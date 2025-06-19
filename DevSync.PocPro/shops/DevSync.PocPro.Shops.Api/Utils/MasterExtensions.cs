using DevSync.PocPro.Shops.Shared.Interfaces;

namespace DevSync.PocPro.Shops.Api.Utils;

public class MasterExtensions(MainShopTemplateDbContext accountsDbContext, ITenantServices services) 
    : IMasterExtensions
{
    public async Task ApplyMigrationsAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await services.GetAllTenantsAsync();

        var enumerable = tenants as Tenant[] ?? tenants.ToArray();
        if (enumerable.Length == 0) return;

        foreach (var tenant in enumerable)
        {
            accountsDbContext.Database.SetConnectionString(tenant.ConnectionString);
            await accountsDbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}