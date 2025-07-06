namespace DevSync.PocPro.Shops.StocksModule.Services;

public class ProductServices(IShopDbContext shopDbContext) : IProductServices
{
    public async Task<ProductDto?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await shopDbContext
            .Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == ProductId.Of(productId), cancellationToken);
        
        return product is null ? 
            null : 
            new ProductDto(product.Id.Value, product.Name, product.PhotoUrl ?? string.Empty);
    }
}