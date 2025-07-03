namespace DevSync.PocPro.Shops.StocksModule.Features.Products.UpdateProduct;

public record UpdateProductRequest(
    [FromRoute] Guid ProductId,
    string Name,
    string? BarcodeNumber,
    string? PhotoUrl,
    Guid CategoryId,
    string? Description,
    int LowThresholdValue);