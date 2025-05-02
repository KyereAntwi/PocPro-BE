namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public record GetProductDetailsRequest([FromRoute] Guid ProductId);

public record GetProductDetailsResponse(GetProductDetailsResponseItem Product, StockItem[] Stocks);

public record GetProductDetailsResponseItem(
    Guid Id,
    string Name,
    string ImageUrl,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt);

public record StockItem(
    Guid Id,
    int QuantityPurchased,
    int QuantityLeftInStock,
    decimal CostPerPrice,
    decimal SellingPerPrice,
    decimal TaxRate,
    DateTimeOffset ExpiresAt)
{
    public SupplierItem? Supplier { get; set; } = null!;
};

public record SupplierItem(Guid Id, string Title, string Email)
{
    public IEnumerable<SupplierContactItem> Contact { get; set; } = [];
}

public record SupplierContactItem(string Value, string Type);