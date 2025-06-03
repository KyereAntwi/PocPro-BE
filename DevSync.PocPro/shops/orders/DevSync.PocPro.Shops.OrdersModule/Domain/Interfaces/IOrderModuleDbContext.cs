namespace DevSync.PocPro.Shops.OrdersModule.Domain.Interfaces;

public interface IOrderModuleDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<ShippingAddress> ShippingAddresses { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}