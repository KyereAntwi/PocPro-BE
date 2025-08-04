namespace DevSync.PocPro.Shops.StocksModule.Services;

public class ProductServices(IShopDbContext shopDbContext) : IProductServices
{
    public async Task<ProductDto?> GetProductByIdAsync(Guid productId, Guid posId, CancellationToken cancellationToken = default)
    {
        var product = await shopDbContext
            .Products
            .Include(p => p.Stocks)
            .Where(p => p.Id == ProductId.Of(productId))
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        return product is null
            ? null
            : new ProductDto(
                product.Id.Value,
                product.Name,
                product.PhotoUrl ?? string.Empty,
                product.CurrentSellingPrice(posId != Guid.Empty ? PointOfSaleId.Of(posId) : null))
            {
                PosId = posId
            };
    }

    public async Task<string?> GetBrandNameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await shopDbContext
            .Brands
            .Where(b => b.Id == BrandId.Of(id))
            .Select(b => b.Title)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> GetCategoryNameByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await shopDbContext
            .Categories
            .Where(c => c.Id == CategoryId.Of(id))
            .Select(c => c.Title)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
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
            var product = await query.Where(p => p.IsLowInStock(id)).FirstOrDefaultAsync(cancellationToken);
            if (product is not null)
            {
                products.Add(new ProductDto(product.Id.Value, product.Name, product.PhotoUrl ?? string.Empty, 0)
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
            var product = await query.Where(p => p.IsExpiredWithinAWeek(id)).FirstOrDefaultAsync(cancellationToken);
            if (product is not null)
            {
                products.Add(new ProductDto(product.Id.Value, product.Name, product.PhotoUrl ?? string.Empty, product.CurrentSellingPrice(id))
                {
                    PosId = id.Value
                });
            }
        }
        
        return products;
    }

    public async Task<(decimal, decimal)> GetTotalAmountOfStocksLeftAsync(
        PointOfSaleId? pointOfSaleId, DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        var totalAmountQuery = shopDbContext
            .Stocks
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate && s.QuantityLeftInStock > 0)
            .AsNoTracking();
        
        var totalForPreviousPeriodQuery = shopDbContext
            .Stocks
            .Where(s => s.CreatedAt >= startDate.AddMonths(-1) && s.CreatedAt < startDate && s.QuantityLeftInStock > 0)
            .AsNoTracking();

        if (pointOfSaleId is not null)
        {
            totalAmountQuery = totalAmountQuery.Where(s => s.PointOfSaleId == pointOfSaleId);
            totalForPreviousPeriodQuery = totalForPreviousPeriodQuery.Where(s => s.PointOfSaleId == pointOfSaleId);
        }

        var totalAmount =
            await totalAmountQuery.SumAsync(s => (s.QuantityLeftInStock * s.SellingPerPrice), cancellationToken);

        var totalForPreviousPeriod = await totalForPreviousPeriodQuery
            .SumAsync(s => (s.QuantityLeftInStock * s.SellingPerPrice), cancellationToken);
        
        var percentageChange = totalForPreviousPeriod == 0
            ? 0
            : ((totalAmount - totalForPreviousPeriod) / totalForPreviousPeriod) * 100;
        
        return (totalAmount, percentageChange);
    }
}