namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Product : BaseEntity<ProductId>
{
    private readonly Collection<Stock> _stocks = [];
    public IReadOnlyCollection<Stock> Stocks => _stocks;
    
    public static Product Create(string name, string barcodeNumber, string photoUrl, CategoryId categoryId)
    {
        var product = new Product
        {
            Name = name,
            BarcodeNumber = barcodeNumber,
            PhotoUrl = photoUrl,
            CategoryId = categoryId
        };
        
        return product;
    }

    public void Update(string name, string barcodeNumber, string photoUrl, CategoryId categoryId)
    {
        Name = name;
        BarcodeNumber = barcodeNumber;
        PhotoUrl = photoUrl;
        CategoryId = categoryId;
    }

    public Result StockProduct(Guid supplierId, int quantityPurchased, int quantityLeftInStock, decimal costPrice, decimal sellingPrice,
        decimal taxRate, DateTimeOffset expirationDate)
    {
        if (DateTime.Now.Date > expirationDate.Date)
        {
            return Result.Fail("This stock's products are expired");
        }
        
        _stocks.Add(new Stock(supplierId, Id, quantityPurchased, quantityLeftInStock, costPrice, sellingPrice, taxRate, expirationDate));
        return Result.Ok();
    }

    public Result MakePurchase(int quantity)
    {
        if (_stocks.Count == 0)
        {
            return Result.Fail("There are no stocks for product");
        }

        if (!_stocks.Select(s => s.QuantityLeftInStock).Any())
        {
            return Result.Fail("There are no products left to purchase from");
        }

        var recentStock = _stocks.Last();
        recentStock.MakePurchase(quantity);
        
        return Result.Ok();
    }

    public decimal ExpectedProfitAfterPurchasesOnProduct()
    {
        return _stocks.Count <= 0 ? 0 : 
            (from stock in _stocks 
                let capital = stock.QuantityPurchased * stock.CostPerPrice 
                let totalInPurchase = (stock.QuantityPurchased - stock.QuantityLeftInStock) * stock.SellingPerPrice 
                select (totalInPurchase - capital))
            .Sum();
    }

    public decimal ExpectedTotalProfitOnProduct()
    {
        return _stocks.Count <= 0
            ? 0
            : (from stock in _stocks
                let capital = stock.QuantityPurchased * stock.CostPerPrice
                let totalInPurchase = stock.QuantityPurchased * stock.SellingPerPrice
                select (totalInPurchase - capital))
            .Sum();
    }

    public string Name { get; private set; } = string.Empty;
    public string? BarcodeNumber { get; private set; }
    public string? PhotoUrl { get; private set; }
    public CategoryId CategoryId { get; private set; }
}