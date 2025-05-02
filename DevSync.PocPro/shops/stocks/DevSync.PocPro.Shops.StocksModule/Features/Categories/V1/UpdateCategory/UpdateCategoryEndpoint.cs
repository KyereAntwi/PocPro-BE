namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.UpdateCategory;

public class UpdateCategoryEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<UpdateCategoryRequest>
{
    public override void Configure()
    {
        Put("/categories/{Id}");
    }

    public override async Task HandleAsync(UpdateCategoryRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_CATEGORIES, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var existingCategory = await shopDbContext.Categories
            .FindAsync(CategoryId.Of(req.Id), ct);

        if (existingCategory == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        existingCategory.Update(req.Title, req.Description, req.IsActive);
        shopDbContext.Categories.Update(existingCategory);
        await shopDbContext.SaveChangesAsync(ct);
        
        await SendAsync(new BaseResponse<string>("Category updated successfully", true), cancellation: ct);
    }
}