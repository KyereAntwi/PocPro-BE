using DevSync.PocPro.Shops.Shared.Dtos;

namespace DevSync.PocPro.Shops.Shared.Interfaces;

public interface IProductServices
{
    Task<ProductDto?> GetProductByIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default);
}