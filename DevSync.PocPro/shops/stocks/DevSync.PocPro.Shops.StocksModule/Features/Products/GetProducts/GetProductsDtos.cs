namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProducts;

public record GetProductsRequests(
    int Page,
    int PageSize,
    string? SearchText,
    Guid? CategoryId
);

public record GetProductsResponse(IEnumerable<GetProductsResponse> Products);

public record GetProductsResponseItem(
    Guid Id,
    string Name,
    decimal Price,
    string? ImageUrl,
    Guid CategoryId
);