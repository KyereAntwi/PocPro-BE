namespace DevSync.PocPro.Shops.PointOfSales.Domains.Interfaces;

public interface IPOSDbContext
{
    DbSet<Session> Sessions { get; }
    DbSet<PointOfSale> PointOfSales { get; }
    DbSet<PointOfSaleManager> PointOfSaleManagers { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}