using DevSync.PocPro.Shops.Shared.Dtos;
using DevSync.PocPro.Shops.Shared.ValueObjects;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IProductServices
{
    Task<ProductDto?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ProductDto>> GetProductsLowInStockAsync(
        IEnumerable<PointOfSaleId> posIds,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ProductDto>> GetProductsExpiringSoonAsync(
        IEnumerable<PointOfSaleId> posIds,
        CancellationToken cancellationToken = default);
}