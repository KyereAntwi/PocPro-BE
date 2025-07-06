namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.GetBrand;

public record GetBrandRequest(Guid Id);

public record BrandResponse(
    Guid Id,
    string Title,
    string? Description,
    string? ImageUrl
);