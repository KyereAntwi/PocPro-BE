namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.UpdateCategory;

public record UpdateCategoryRequest([FromRoute] Guid Id, string Title, string? Description, bool IsActive);