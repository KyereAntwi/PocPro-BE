namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Product : BaseEntity<ProductId>
{
    private readonly Collection<Stock> _stocks = [];
    public IReadOnlyCollection<Stock> Stocks => _stocks;

    private readonly Collection<ProductMedia> _media = [];
    public IReadOnlyCollection<ProductMedia> Media => _media;
    
    public static Product Create(
        string name, string barcodeNumber, string photoUrl, CategoryId categoryId, string? description, int lowThresholdValue, IEnumerable<ProductMedia> media)
    {
        var product = new Product
        {
            Id = ProductId.Of(Guid.CreateVersion7()),
            Name = name,
            BarcodeNumber = barcodeNumber,
            PhotoUrl = photoUrl,
            CategoryId = categoryId,
            Description = description,
            LowThresholdValue = lowThresholdValue
        };
        
        foreach (var item in media)
        {
            product._media.Add(item);
        }

        return product;
    }

    public void Update(string name, string barcodeNumber, string photoUrl, CategoryId categoryId, string? description, int lowThresholdValue)
    {
        Name = name;
        BarcodeNumber = barcodeNumber;
        PhotoUrl = photoUrl;
        CategoryId = categoryId;
        Description = description;
        LowThresholdValue = lowThresholdValue;
    }

    public Result<Guid> StockProduct(Supplier supplier, PointOfSaleId pointOfSaleId, int quantityPurchased, int quantityLeftInStock, decimal costPrice, decimal sellingPrice,
        decimal taxRate, DateTimeOffset expirationDate)
    {
        if (DateTime.Now.Date > expirationDate.Date)
        {
            return Result.Fail("This stock's products are expired");
        }

        var newStock = new Stock(
            supplier, Id, pointOfSaleId, quantityPurchased, quantityLeftInStock, costPrice, sellingPrice, taxRate,
            expirationDate);
        
        _stocks.Add(newStock);
        return Result.Ok(newStock.Id.Value);
    }

    public Result MakePurchase(int quantity, PointOfSaleId pointOfSaleId)
    {
        var stocks = _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        
        if (stocks.Length == 0)
        {
            return Result.Fail($"There are no stocks for product and given POS with Id {pointOfSaleId.Value}");
        }

        if (!stocks.Select(s => s.QuantityLeftInStock).Any())
        {
            return Result.Fail("There are no products left to purchase from");
        }
        
        // make purchase on the latest stock
        stocks.OrderBy(s => s.CreatedAt).Last().MakePurchase(quantity);
        
        return Result.Ok();
    }

    public int TotalNumberLeftOnShelf(PointOfSaleId pointOfSaleId)
    {
        var stocks = _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        return stocks.Length > 0 ?  stocks.Sum(s => s.QuantityLeftInStock) : 0;
    }

    public decimal CurrentSellingPrice(PointOfSaleId? pointOfSaleId = null)
    {
        var stocks = pointOfSaleId == null 
            ? _stocks.ToArray() 
            : _stocks.Where(stock => stock.PointOfSaleId == pointOfSaleId).ToArray();
        
        return stocks.Length > 0 ? stocks.OrderBy(s => s.CreatedAt).Last().SellingPerPrice : 0;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int LowThresholdValue { get; private set; }
    public string? BarcodeNumber { get; private set; }
    public string? PhotoUrl { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public BrandId? BrandId { get; private set; }
}