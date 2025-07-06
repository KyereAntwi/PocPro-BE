namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.UpdateBrand;

public record UpdateBrandRequest(Guid Id, string Title, string? Description, string? ImageUrl);