namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetCategory;

public record GetCategoryRequest(Guid Id);

public record GetCategoryResponse(string Title, string Description, string Status, Guid Id, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt, string ImageUrl);