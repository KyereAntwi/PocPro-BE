namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductsForOnlineSite;

public record GetProductsForOnlineSiteRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SearchText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string BrandIds { get; set; } = string.Empty;
}