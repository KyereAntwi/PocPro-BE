using DevSync.PocPro.Shops.Shared.Interfaces;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevSync.PocPro.Shops.Api.Utils;

public static class ApplicationExtensions
{
    public static async Task ConfigureDatabaseAsync(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        try
        {
            var accountsDbContext = scope.ServiceProvider.GetRequiredService<MainShopTemplateDbContext>();
            var tenantServices = scope.ServiceProvider.GetRequiredService<ITenantServices>();

            //await EnsureDatabaseExistAsync(accountsDbContext);
            await ApplyMigrationsAsync(accountsDbContext, tenantServices);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private static async Task EnsureDatabaseExistAsync(MainShopTemplateDbContext accountsDbContext)
    {
        var dbCreator = accountsDbContext.GetService<IRelationalDatabaseCreator>();
        var strategy = accountsDbContext.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync())
            {
                await dbCreator.CreateAsync();
            }
        });
    }
    
    private static async Task ApplyMigrationsAsync(MainShopTemplateDbContext accountsDbContext, ITenantServices services)
    {
        var tenants = await services.GetAllTenantsAsync();

        var enumerable = tenants as Tenant[] ?? tenants.ToArray();
        if (enumerable.Length == 0) return;
        
        var strategy = accountsDbContext.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await accountsDbContext.Database.BeginTransactionAsync();
            
            try
            {
                foreach (var tenant in enumerable)
                {
                    accountsDbContext.Database.SetConnectionString(tenant.ConnectionString);
                    await accountsDbContext.Database.MigrateAsync();
                }
                
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }
}