namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProducts;

public record GetProductsRequests
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchText { get; set; } = string.Empty;
    public Guid CategoryId { get; set; } = Guid.Empty;
}

public record GetProductsResponse(IEnumerable<GetProductsResponse> Products);

public record GetProductsResponseItem(
    Guid Id,
    string Name,
    decimal? Price,
    string? ImageUrl,
    Guid CategoryId
);