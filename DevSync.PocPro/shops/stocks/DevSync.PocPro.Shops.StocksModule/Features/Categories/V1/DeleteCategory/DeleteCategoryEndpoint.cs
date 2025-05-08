using DevSync.PocPro.Shops.StocksModule.Events;
using MassTransit;

namespace DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.DeleteCategory;

public class DeleteCategoryEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices, IPublishEndpoint publishEndpoint) 
    : Endpoint<DeleteCategoryRequest>
{
    public override void Configure()
    {
        Delete("/categories/{Id}");
    }

    public override async Task HandleAsync(DeleteCategoryRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_CATEGORIES, userId!))
        {
            await SendForbiddenAsync(cancellation: ct);
        }
        
        var category = await shopDbContext.Categories.FindAsync(CategoryId.Of(req.Id), ct);
        
        if (category is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        shopDbContext.Categories.Remove(category);
        await shopDbContext.SaveChangesAsync(ct);
        
        // raise event to update all products in that category
        try
        {
            await publishEndpoint.Publish(new UpdateProductsCategoryIdEvent
            {
                CategoryId = category.Id.Value
            }, ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}