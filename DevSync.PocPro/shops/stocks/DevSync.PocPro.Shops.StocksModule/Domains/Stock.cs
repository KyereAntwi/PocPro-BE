namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Stock : BaseEntity<StockId>
{
    private Stock() {}

    internal Stock(
        Supplier supplier, ProductId productId, PointOfSaleId pointOfSaleId, int quantityPurchased, int quantityLeftInStock, 
        decimal costPrice, decimal sellingPerPrice, decimal taxRate, DateTimeOffset expirationDate)
    {
        Id = StockId.Of(Guid.CreateVersion7());
        ProductId = productId;
        QuantityPurchased = quantityPurchased;
        QuantityLeftInStock = quantityLeftInStock;
        CostPerPrice = costPrice;
        SellingPerPrice = sellingPerPrice;
        TaxRate = taxRate;
        ExpiresAt = expirationDate;
        Supplier = supplier;
        PointOfSaleId = pointOfSaleId;
    }

    internal void MakePurchase(int quantity)
    {
        QuantityLeftInStock -= quantity;
    }

    public Supplier Supplier { get; private set; }
    public ProductId ProductId { get; private set; }
    public int QuantityPurchased { get; private set; }
    public int QuantityLeftInStock { get; private set; }
    public decimal CostPerPrice { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal SellingPerPrice { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public PointOfSaleId PointOfSaleId { get; private set; }
}