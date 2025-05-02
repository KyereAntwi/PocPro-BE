namespace DevSync.PocPro.Shops.PointOfSales.Domains.Interfaces;

public interface IPOSDbContext
{
    DbSet<Session> Sessions { get; }
    DbSet<PointOfSale> PointOfSales { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}