using DevSync.PocPro.Shops.Shared.Dtos;

namespace DevSync.PocPro.WorkerService.Interfaces;

public interface IProductServices
{
    Task<IReadOnlyList<ProductDto>> GetProductsLowInStockAsync(List<Guid> posIds, string identifier, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProductDto>> GetProductsExpiringSoonAsync(List<Guid> posIds, string identifier, CancellationToken cancellationToken = default);
}