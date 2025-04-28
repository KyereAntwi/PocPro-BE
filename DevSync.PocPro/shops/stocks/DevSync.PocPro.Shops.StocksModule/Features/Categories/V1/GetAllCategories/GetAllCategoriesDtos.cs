namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetAllCategories;

public record GetAllCategoriesResponse(IEnumerable<GetCategoryResponse> Categories);