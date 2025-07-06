namespace DevSync.PocPro.Shops.StocksModule.Features.Brands.V1.UpdateBrand;

public class UpdateBrandEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) : Endpoint<UpdateBrandRequest>
{
    public override void Configure()
    {
        Put("/api/v1/brands/{Id}");
        Description(x => x
            .WithName("UpdateBrand")
            .WithSummary("Update a brand")
            .WithTags("Brands"));
    }

    public override async Task HandleAsync(UpdateBrandRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_BRANDS, userId!))
        {
            await SendAsync(new BaseResponse<Guid>("You do not have permission to create a brand." , false)
            {
                Errors = new List<string> { "Forbidden" }
            }, (int)HttpStatusCode.Forbidden , ct);
            return;
        }
        
        var existingBrand = await shopDbContext.Brands.FindAsync(BrandId.Of(req.Id), ct);
        
        if (existingBrand == null)
        {
            await SendAsync(new BaseResponse<Guid>("Operation failed" , false)
            {
                Errors = new List<string> { $"Brand with ID {req.Id} does not exist." }
            }, (int)HttpStatusCode.NotFound , ct);
            return;
        }
        
        existingBrand.Update(req.Title, req.Description, StatusType.Active, req.ImageUrl);
        await shopDbContext.SaveChangesAsync(ct);
        await SendNoContentAsync(ct);
    }
}