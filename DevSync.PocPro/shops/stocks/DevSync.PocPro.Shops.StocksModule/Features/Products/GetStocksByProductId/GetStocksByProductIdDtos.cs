namespace DevSync.PocPro.Shops.StocksModule.Features.Products.GetStocksByProductId;

public record GetStocksByProductIdRequest(
    [FromRoute] Guid ProductId,
    int Page = 1,
    int PageSize = 20);