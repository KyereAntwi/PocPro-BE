namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.AddCategory;

public class AddCategoryEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<AddCategoryRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/categories");
    }

    public override async Task HandleAsync(AddCategoryRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_CATEGORIES, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var existingCategory = await shopDbContext.Categories.FirstOrDefaultAsync(c => c.Title.ToLower() == req.Title.ToLower(), ct);

        if (existingCategory != null)
        {
            await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }
        
        var newCategory = Category.Create(req.Title, req.Description);
        await shopDbContext.Categories.AddAsync(newCategory, ct);
        await shopDbContext.SaveChangesAsync(ct);
        
        await SendCreatedAtAsync<GetCategoryEndpoint>(new { Id = newCategory.Id }, new BaseResponse<Guid>("Category created successfully", true)
        {
            Data = newCategory.Id.Value
        }, cancellation: ct);
    }
}

public class AddCategoryRequestValidator : Validator<AddCategoryRequest>
{
    public AddCategoryRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull();
    }
}