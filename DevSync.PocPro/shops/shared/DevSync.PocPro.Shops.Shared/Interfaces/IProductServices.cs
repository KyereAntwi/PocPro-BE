using DevSync.PocPro.Shops.Shared.Dtos;
using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IProductServices
{
    Task<ProductDto?> GetProductByIdAsync(
        Guid productId,
        Guid posId,
        CancellationToken cancellationToken = default);
    
    Task<string?> GetBrandNameByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string?> GetCategoryNameByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ProductDto>> GetProductsLowInStockAsync(
        IEnumerable<PointOfSaleId> posIds,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ProductDto>> GetProductsExpiringSoonAsync(
        IEnumerable<PointOfSaleId> posIds,
        CancellationToken cancellationToken = default);
    
    Task<(decimal, decimal)> GetTotalAmountOfStocksLeftAsync(
        PointOfSaleId? pointOfSaleId,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        CancellationToken cancellationToken = default);
}