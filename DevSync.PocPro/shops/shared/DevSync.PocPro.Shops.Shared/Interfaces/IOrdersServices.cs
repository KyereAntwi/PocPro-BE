using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IOrdersServices
{
    Task<IReadOnlyList<ProductId>> GetMostPerformingProductsAsync(
        int size, 
        bool forOnlineSite, 
        CancellationToken cancellationToken = default);
    
    Task<(decimal, decimal)> GetTotalSalesAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset from, 
        DateTimeOffset to,
        string? paymentMethod,
        CancellationToken cancellationToken = default);
    
    Task<(int, decimal)> GetTotalPendingOrdersAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset startDate, 
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);
    
    Task<Dictionary<int, (decimal, decimal)>> GetMonthlyGroupedSalesAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);

    Task<Dictionary<string, decimal>> GetGroupedSalesByPaymentMethodAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);
    
    Task<int> GetTotalSalesCountAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset startDate, 
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);
}