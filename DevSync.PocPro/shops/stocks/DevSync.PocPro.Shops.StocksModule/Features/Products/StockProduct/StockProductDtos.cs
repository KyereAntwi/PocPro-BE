namespace DevSync.PocPro.Shops.StocksModule.Features.Products.StockProduct;

public record StockProductRequest(
    Guid ProductId,
    Guid SupplierId,
    int QuantityPurchased,
    decimal CostPerPrice,
    decimal SellingPerPrice,
    decimal TaxRate,
    DateTime ExpiryAt);

public record StockProductResponse(Guid StockId);