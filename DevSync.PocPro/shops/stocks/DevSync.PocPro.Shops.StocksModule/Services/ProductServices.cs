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

    public async Task<IEnumerable<ProductDto>> GetProductsLowInStockAsync(IEnumerable<PointOfSaleId> posIds, CancellationToken cancellationToken = default)
    {
        var query = shopDbContext
            .Products
            .Include(p => p.Stocks)
            .AsSplitQuery()
            .AsNoTracking();
        
        List<ProductDto> products = [];

        foreach (var id in posIds)
        {
            var product = await query.Where(p => p.IsLowInStock(id)).FirstAsync(cancellationToken);
            if (product is not null)
            {
                products.Add(new ProductDto(product.Id.Value, product.Name, product.PhotoUrl ?? string.Empty)
                {
                    PosId = id.Value
                });
            }
        }
        
        return products;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsExpiringSoonAsync(IEnumerable<PointOfSaleId> posIds, CancellationToken cancellationToken = default)
    {
        var query = shopDbContext
            .Products
            .Include(p => p.Stocks)
            .AsSplitQuery()
            .AsNoTracking();
        
        List<ProductDto> products = [];

        foreach (var id in posIds)
        {
            var product = await query.Where(p => p.IsExpiredWithinAWeek(id)).FirstAsync(cancellationToken);
            if (product is not null)
            {
                products.Add(new ProductDto(product.Id.Value, product.Name, product.PhotoUrl ?? string.Empty)
                {
                    PosId = id.Value
                });
            }
        }
        
        return products;
    }
}