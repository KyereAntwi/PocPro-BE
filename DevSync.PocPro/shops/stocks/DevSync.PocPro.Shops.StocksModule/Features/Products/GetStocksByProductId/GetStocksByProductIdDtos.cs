namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetStocksByProductId;

public record GetStocksByProductIdRequest(
    [FromRoute] Guid ProductId,
    string Pos = "",
    int Page = 1,
    int PageSize = 20);