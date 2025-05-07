namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetCategory;

public class GetCategoryEndpoint(IShopDbContext shopDbContext) 
    : Endpoint<GetCategoryRequest, BaseResponse<GetCategoryResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/categories/{Id}");
    }

    public override async Task HandleAsync(GetCategoryRequest req, CancellationToken ct)
    {
        var category = await shopDbContext.Categories.FindAsync(CategoryId.Of(req.Id), ct);

        if (category is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetCategoryResponse>("Category", true)
        {
            Data = new GetCategoryResponse(category.Title, category.Description!, category.Status, category.Id.Value, category.CreatedAt, category.UpdatedAt)
        }, cancellation: ct);
    }
}

public class GetCategoryRequestValidator : Validator<GetCategoryRequest>
{
    public GetCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull();
    }
}