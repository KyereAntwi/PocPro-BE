namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public record GetProductDetailsRequest([FromRoute] Guid ProductId);

public record GetProductDetailsResponseItem(
    Guid Id,
    string Name,
    string Barcode,
    string ImageUrl,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? CategoryId,
    int TotalNumberOfStocks,
    decimal Price,
    int NumberLeftOnShelf,
    string Description,
    int LowThresholdValue)
{
    public IEnumerable<MediaItemResponse> ProductMedia { get; set; } = [];
}

public record MediaItemResponse(string Url, string MediaType);

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