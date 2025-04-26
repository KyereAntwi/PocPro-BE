using Microsoft.EntityFrameworkCore.Design;

namespace DevSync.PocPro.Accounts.Api.Data;

public class AccountsDbContextFactory : IDesignTimeDbContextFactory<AccountsDbContext>
{
    public AccountsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AccountsDbContext>();
        optionsBuilder.UseNpgsql("PocProAccountsManagement"); // Use PostgreSQL provider

        // Return the DbContext without IHttpContextAccessor for design-time
        return new AccountsDbContext(optionsBuilder.Options, null!);
    }
}