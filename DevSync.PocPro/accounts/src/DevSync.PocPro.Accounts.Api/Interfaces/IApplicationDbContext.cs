namespace DevSync.PocPro.Accounts.Api.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> ApplicationUsers { get; }
    DbSet<Tenant> Tenants { get; }
    DbSet<Permission> Permissions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}