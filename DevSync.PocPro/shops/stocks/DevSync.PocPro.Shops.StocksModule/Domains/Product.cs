namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Product : BaseEntity<ProductId>
{
    private readonly Collection<Stock> _stocks = [];
    public IReadOnlyCollection<Stock> Stocks => _stocks;

    private readonly Collection<ProductMedia> _media = [];
    public IReadOnlyCollection<ProductMedia> Media => _media;
    
    public static Product Create(
        string name, string barcodeNumber, string photoUrl, CategoryId categoryId, string? description, int lowThresholdValue, 
        IEnumerable<ProductMedia> media, bool isFeatured = false, BrandId? brandId = null)
    {
        var product = new Product
        {
            Id = ProductId.Of(Guid.CreateVersion7()),
            Name = name,
            BarcodeNumber = barcodeNumber,
            PhotoUrl = photoUrl,
            CategoryId = categoryId,
            Description = description,
            LowThresholdValue = lowThresholdValue,
            IsFeatured = isFeatured,
            BrandId = brandId,
        };
        
        foreach (var item in media)
        {
            product._media.Add(item);
        }

        return product;
    }

    public void Update(string name, string barcodeNumber, string photoUrl, CategoryId categoryId, string? description, int lowThresholdValue, bool isFeatured = false, BrandId? brandId = null)
    {
        Name = name;
        BarcodeNumber = barcodeNumber;
        PhotoUrl = photoUrl;
        CategoryId = categoryId;
        Description = description;
        LowThresholdValue = lowThresholdValue;
        IsFeatured = isFeatured;
        BrandId = brandId;
    }

    public Result<Guid> StockProduct(
        Supplier supplier, PointOfSaleId pointOfSaleId, int quantityPurchased, decimal costPrice, decimal sellingPrice, decimal taxRate, DateTimeOffset expirationDate)
    {
        if (DateTime.Now.Date > expirationDate.Date)
        {
            return Result.Fail("This stock's products are expired");
        }

        var totalQuantityLeftInStock = 0;
        
        if (_stocks.Count > 0)
        {
            var lastStock = _stocks.OrderByDescending(s => s.CreatedAt).First();
            totalQuantityLeftInStock = lastStock.QuantityLeftInStock + quantityPurchased;
            
            lastStock.ResetQuantityLeftInStock();
        }
        else
        {
            totalQuantityLeftInStock = quantityPurchased;
        }

        var newStock = new Stock(
            supplier, Id, pointOfSaleId, quantityPurchased, totalQuantityLeftInStock, costPrice, sellingPrice, taxRate,
            expirationDate);
        
        _stocks.Add(newStock);
        return Result.Ok(newStock.Id.Value);
    }

    public Result MakePurchase(int quantity, PointOfSaleId pointOfSaleId)
    {
        var stocks = _stocks
            .Where(stock => stock.PointOfSaleId == pointOfSaleId)
            .OrderByDescending(s => s.CreatedAt)
            .ToArray();
        
        if (stocks.Length == 0)
        {
            return Result.Fail($"There are no stocks for product and given POS with Id {pointOfSaleId.Value}");
        }
        
        if (stocks.First().QuantityLeftInStock < quantity)
        {
            return Result.Fail("There are no products left to purchase from or the quantity requested is more than available in stock");
        }
        
        stocks.First().MakePurchase(quantity);
        
        return Result.Ok();
    }
    
    public Result ReversePurchase(int quantity, PointOfSaleId pointOfSaleId)
    {
        var stocks = _stocks
            .Where(stock => stock.PointOfSaleId == pointOfSaleId)
            .OrderByDescending(s => s.CreatedAt)
            .ToArray();
        
        if (stocks.Length == 0)
        {
            return Result.Fail($"There are no stocks for product and given POS with Id {pointOfSaleId.Value}");
        }
        
        stocks.First().ReversePurchase(quantity);
        
        return Result.Ok();
    }

    public int TotalNumberLeftOnShelf(PointOfSaleId? pointOfSaleId = null)
    {
        var stocks = pointOfSaleId == null 
            ? _stocks.ToArray() 
            : _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();

        if (stocks.Length == 0)
        {
            return 0;
        }

        var mostRecentStock = stocks.OrderByDescending(s => s.CreatedAt).First();
        return mostRecentStock.QuantityLeftInStock;
    }

    public decimal CurrentSellingPrice(PointOfSaleId? pointOfSaleId = null)
    {
        var stocks = pointOfSaleId == null 
            ? _stocks.ToArray() 
            : _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        
        return stocks.Length > 0 ? stocks.OrderBy(s => s.CreatedAt).Last().SellingPerPrice : 0;
    }
    
    public bool IsLowInStock(PointOfSaleId? pointOfSaleId = null)
    {
        var stocks = pointOfSaleId == null 
            ? _stocks.ToArray() 
            : _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        
        return stocks.Length > 0 && stocks.Sum(s => s.QuantityLeftInStock) <= LowThresholdValue;
    }

    public bool IsExpiredWithinAWeek(PointOfSaleId? pointOfSaleId = null)
    {
        var stocks = pointOfSaleId == null 
            ? _stocks.ToArray() 
            : _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        
        return stocks.Length > 0 && stocks.Any(s => s.ExpiresAt.Date <= DateTime.Now.AddDays(7).Date);
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int LowThresholdValue { get; private set; }
    public string? BarcodeNumber { get; private set; }
    public string? PhotoUrl { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public BrandId? BrandId { get; private set; }
    public bool IsFeatured { get; private set; }
}