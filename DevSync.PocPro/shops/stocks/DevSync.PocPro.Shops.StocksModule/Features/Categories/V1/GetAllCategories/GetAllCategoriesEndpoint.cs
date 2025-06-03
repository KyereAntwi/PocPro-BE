namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetAllCategories;

public class GetAllCategoriesEndpoint(IShopDbContext shopDbContext) 
    : EndpointWithoutRequest<BaseResponse<IEnumerable<GetCategoryResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/categories");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var categories = await shopDbContext
            .Categories
            .Select(c => 
                new GetCategoryResponse(
                    c.Title, 
                    c.Description ?? "", 
                    c.Status ? "Active" : "Inactive", 
                    c.Id.Value, 
                    c.CreatedAt, 
                    c.UpdatedAt))
            .AsNoTracking()
            .ToListAsync(cancellationToken: ct);
        
        await SendOkAsync(new BaseResponse<IEnumerable<GetCategoryResponse>>("Categories fetched successfully", true)
        {
            Data = categories
        }, cancellation: ct);
    }
}