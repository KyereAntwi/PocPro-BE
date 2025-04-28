namespace DevSync.PocPro.Shops.StocksModule.Domains.Interfaces;

public interface IShopDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Contact> Contacts { get; }
    DbSet<Product> Products { get; }
    DbSet<Stock> Stocks { get; }
    DbSet<Supplier> Suppliers { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}