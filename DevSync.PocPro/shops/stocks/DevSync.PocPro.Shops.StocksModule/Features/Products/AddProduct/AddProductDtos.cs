namespace DevSync.PocPro.Shops.StocksModule.Features.Products.AddProduct;

public record AddProductRequest(
    string Name,
    string? BarcodeNumber,
    IFormFile? ImageFile,
    Guid CategoryId);

public record AddProductResponse(Guid Id);