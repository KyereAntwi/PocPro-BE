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

    public async Task<(decimal, decimal)> GetTotalSalesAsync(
        PointOfSaleId? pointOfSaleId, DateTimeOffset from, DateTimeOffset to, string? paymentMethod, CancellationToken cancellationToken = default)
    {
        var totalSalesQuery = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Delivered && x.CreatedAt >= from && x.CreatedAt <= to);
        
        var totalSalesForPreviousPeriodQuery = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Delivered && x.CreatedAt >= from.AddMonths(-1) && x.CreatedAt < from);

        if (pointOfSaleId is not null)
        {
            totalSalesQuery = totalSalesQuery.Where(o => o.PointOfSaleId == pointOfSaleId);
            totalSalesForPreviousPeriodQuery = totalSalesForPreviousPeriodQuery.Where(o => o.PointOfSaleId == pointOfSaleId);      
        }

        if (!string.IsNullOrWhiteSpace(paymentMethod))
        {
            Enum.TryParse<PaymentMethod>(paymentMethod, out var result);

            if (result is PaymentMethod.MobileMoney)
            {
                totalSalesQuery = totalSalesQuery.Where(o => o.PaymentMethod == PaymentMethod.MobileMoney);
                totalSalesForPreviousPeriodQuery = totalSalesForPreviousPeriodQuery.Where(o => o.PaymentMethod == PaymentMethod.MobileMoney);      
            }
        }

        var totalSales =
            await totalSalesQuery.SumAsync(x => x.OrderItems.Sum(oi => oi.ProductPrice), cancellationToken);
        var totalSalesForPreviousPeriod = await totalSalesForPreviousPeriodQuery
            .SumAsync(x => x.OrderItems.Sum(oi => oi.ProductPrice), cancellationToken);
        
        var percentageChange = totalSalesForPreviousPeriod == 0
            ? 0
            : ((totalSales - totalSalesForPreviousPeriod) / totalSalesForPreviousPeriod) * 100;
        
        return (totalSales, percentageChange);
    }

    public async Task<(int, decimal)> GetTotalPendingOrdersAsync(
        PointOfSaleId? pointOfSaleId, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var totalPendingOrdersQuery = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Pending && x.CreatedAt >= startDate && x.CreatedAt <= endDate);
        
        var percentageChangeQuery =  ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Pending && x.CreatedAt >= startDate.AddMonths(-1) && x.CreatedAt < startDate);

        if (pointOfSaleId is not null)
        {
            totalPendingOrdersQuery = percentageChangeQuery.Where(o => o.PointOfSaleId == pointOfSaleId);
            percentageChangeQuery = percentageChangeQuery.Where(o => o.PointOfSaleId == pointOfSaleId);       
        }
        
        var totalPendingOrders = await totalPendingOrdersQuery.CountAsync(cancellationToken);
        var percentageChange = await percentageChangeQuery.CountAsync(cancellationToken);
        
        var percentageChangeValue = percentageChange == 0
            ? 0
            : ((decimal)totalPendingOrders - percentageChange) / percentageChange * 100;
        
        return (totalPendingOrders, percentageChangeValue);
    }

    public async Task<Dictionary<int, (decimal, decimal)>> GetMonthlyGroupedSalesAsync(
        PointOfSaleId? pointOfSaleId, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var query = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Delivered && x.CreatedAt >= startDate && x.CreatedAt <= endDate);

        if (pointOfSaleId is not null)
        {
            query = query.Where(x => x.PointOfSaleId == pointOfSaleId);       
        }

        return await query
            .GroupBy(x => x.CreatedAt!.Value.Date.Month)
            .Select(g => new
            {
                Month = g
                    .Key,
                
                TotalForPos = g
                    .Where(x => x.Type == OrderType.PurchaseOrder || x.Type == OrderType.SalesOrder)
                    .Sum(x => x.OrderItems.Sum(oi => oi.ProductPrice)),
                
                TotalForOnline = g
                    .Where(x => x.Type == OrderType.OnlineOrder)
                    .Sum(x => x.OrderItems.Sum(oi => oi.ProductPrice))
            })
            .ToDictionaryAsync(g => g.Month, g => (g.TotalForPos, g.TotalForOnline), cancellationToken);
    }

    public async Task<Dictionary<string, decimal>> GetGroupedSalesByPaymentMethodAsync(PointOfSaleId? pointOfSaleId, DateTimeOffset startDate, DateTimeOffset endDate,
        CancellationToken cancellationToken = default)
    {
        var query = ordersModuleDbContext
            .Orders
            .AsNoTracking()
            .Where(x => x.OrderStatus == OrderStatus.Delivered && x.CreatedAt >= startDate && x.CreatedAt <= endDate);

        if (pointOfSaleId is not null)
        {
            query = query.Where(x => x.PointOfSaleId == pointOfSaleId);       
        }
        
        return await query
            .GroupBy(x => x.PaymentMethod)
            .Select(g => new
            {
                PaymentMethod = g.Key,
                Total = g.Sum(x => x.OrderItems.Sum(oi => oi.ProductPrice))
            })
            .ToDictionaryAsync(g => g.PaymentMethod.ToString(), g => g.Total, cancellationToken);
    }
}