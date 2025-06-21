namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProducts;

public record GetProductsRequests
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Pos { get; set; } = string.Empty;
}

public record GetProductsResponse(IEnumerable<GetProductsResponse> Products);

public record GetProductsResponseItem(
    Guid Id,
    string Name,
    decimal? Price,
    string? ImageUrl,
    Guid CategoryId,
    string Description,
    int LowThresholdValue
)
{
    public GetCategoryResponse? Category { get; set; }
};