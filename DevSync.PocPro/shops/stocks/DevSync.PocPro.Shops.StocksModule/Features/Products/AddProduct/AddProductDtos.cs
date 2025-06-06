namespace DevSync.PocPro.Shops.StocksModule.Features.Products.AddProduct;

public record AddProductRequest(
    string Name,
    string? BarcodeNumber,
    IFormFile? ImageFile,
    Guid CategoryId,
    string? Description,
    int LowThresholdValue);