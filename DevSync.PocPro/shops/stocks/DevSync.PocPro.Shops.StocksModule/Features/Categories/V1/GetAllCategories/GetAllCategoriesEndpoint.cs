namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetAllCategories;

public class GetAllCategoriesEndpoint(IShopDbContext shopDbContext) 
    : EndpointWithoutRequest<BaseResponse<GetAllCategoriesResponse>>
{
    public override void Configure()
    {
        Get("/categories");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var categories = await shopDbContext
            .Categories
            .Select(c => new GetCategoryResponse(c.Title, c.Description, c.Status, c.Id.Value, c.CreatedAt, c.UpdatedAt))
            .ToListAsync(cancellationToken: ct);
        
        await SendOkAsync(new BaseResponse<GetAllCategoriesResponse>("Categories", true)
        {
            Data = new GetAllCategoriesResponse(categories)
        }, cancellation: ct);
    }
}