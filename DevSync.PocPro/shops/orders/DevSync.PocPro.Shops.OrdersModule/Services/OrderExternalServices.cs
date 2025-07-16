namespace DevSync.PocPro.Shops.OrdersModule.Services;

public class OrderExternalServices(OrdersModuleDbContext ordersModuleDbContext) 
    : IOrdersServices
{
    public async Task<IReadOnlyList<ProductId>> GetMostPerformingProductsAsync(int size, bool forOnlineSite, CancellationToken cancellationToken = default)
    {
        var query = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Delivered);

        if (forOnlineSite)
        {
            query = query.Where(x => x.Type == OrderType.OnlineOrder);
        }

        var productIds = await query
            .SelectMany(x => x.OrderItems)
            .GroupBy(x => x.ProductId)
            .OrderByDescending(g => g.Sum(x => x.Quantity))
            .Take(size)
            .Select(g => ProductId.Of(g.Key))
            .ToListAsync(cancellationToken);

        return productIds;
    }
}