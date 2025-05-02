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
    Guid SupplierId,
    int QuantityPurchased,
    int QuantityLeftInStock,
    decimal CostPerPrice,
    decimal SellingPerPrice,
    decimal TaxRate,
    DateTimeOffset ExpiresAt)
{
    public SupplierItem Supplier { get; set; }
};
    
public record SupplierItem(Guid Id, string Title, string Email);