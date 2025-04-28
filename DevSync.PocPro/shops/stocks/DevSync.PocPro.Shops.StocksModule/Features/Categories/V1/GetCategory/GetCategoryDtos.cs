namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetCategory;

public record GetCategoryRequest(Guid Id);

public record GetCategoryResponse(string Title, Guid Id);