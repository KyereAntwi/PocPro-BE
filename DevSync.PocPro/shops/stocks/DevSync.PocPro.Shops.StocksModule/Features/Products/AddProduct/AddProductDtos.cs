namespace DevSync.PocPro.Shops.StocksModule.Features.Products.AddProduct;

public record AddProductRequest(
    string Name,
    string? BarcodeNumber,
    string? PhotoUrl,
    IEnumerable<MediaRequest>? Media,
    Guid CategoryId,
    string? Description,
    int LowThresholdValue);
    
public record MediaRequest(string Url, string MediaType);