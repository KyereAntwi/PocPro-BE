namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.AddCategory;

public record AddCategoryRequest(string Title);

public record AddCategoryResponse(Guid Id);