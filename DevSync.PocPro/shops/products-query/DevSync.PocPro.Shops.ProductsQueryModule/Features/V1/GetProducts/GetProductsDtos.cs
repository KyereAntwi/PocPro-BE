namespace DevSync.PocPro.Shops.ProductsQueryModule.Features.V1.GetProducts;

public record GetProductsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SearchText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
    public string Brand { get; set; } = string.Empty;
    public bool IsFeatured { get; set; } = false;
    public bool IsOnline { get; set; } = true;
    public string Pos { get; set; } = string.Empty;
    public string BarcodeNumber { get; set; } = string.Empty;  
}

public record GetProductsResponseItem(
    Guid Id,
    string Name,
    decimal? Price,
    int QuantityLeft,
    string? PhotoUrl,
    Guid PosId
);