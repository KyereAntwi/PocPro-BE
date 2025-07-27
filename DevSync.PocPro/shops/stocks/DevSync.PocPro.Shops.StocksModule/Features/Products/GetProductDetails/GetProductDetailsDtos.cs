namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

public record GetProductDetailsRequest([FromRoute] Guid ProductId, string Pos = "");

public record GetProductDetailsResponseItem(
    Guid Id,
    string Name,
    string Barcode,
    string photoUrl,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? CategoryId,
    Guid? PosId,
    Guid? BrandId,
    string Description,
    int LowThresholdValue,
    bool IsFeatured)
{
    public decimal Price { get; set; }
    public int QuantityLeft { get; set; }
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
    Guid PosId,
    DateTimeOffset ExpiresAt)
{
    public SupplierItem? Supplier { get; set; } = null!;
};

public record SupplierItem(Guid Id, string Title, string Email)
{
    public IEnumerable<SupplierContactItem> Contact { get; set; } = [];
}

public record SupplierContactItem(string Value, string Type);