namespace DevSync.PocPro.Shops.PrivateCustomers.Interfaces;

public interface ICustomerDbContext
{
    DbSet<Customer> Customers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}