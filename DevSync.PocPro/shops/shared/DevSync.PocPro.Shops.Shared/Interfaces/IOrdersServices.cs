using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IOrdersServices
{
    Task<IReadOnlyList<ProductId>> GetMostPerformingProductsAsync(
        int size, 
        bool forOnlineSite, 
        CancellationToken cancellationToken = default);
}