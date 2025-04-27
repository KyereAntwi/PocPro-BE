using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DevSync.PocPro.Accounts.Api.Utils;

public static class ApplicationExtensions
{
    public static async Task ConfigureDatabaseAsync(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var accountsDbContext = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();
        
        await EnsureDatabaseExistAsync(accountsDbContext);
        await ApplyMigrationsAsync(accountsDbContext);
    }
    
    private static async Task EnsureDatabaseExistAsync(AccountsDbContext accountsDbContext)
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
    
    private static async Task ApplyMigrationsAsync(AccountsDbContext accountsDbContext)
    {
        await accountsDbContext.Database.MigrateAsync();
    }
}