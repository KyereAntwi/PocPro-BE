namespace DevSync.PocPro.Shops.StocksModule.Features.Products.UpdateProduct;

public class UpdateProductEndpoint (
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : Endpoint<UpdateProductRequest>
{
    public override void Configure()
    {
        Put("/api/v1/products/{ProductId}");
    }

    public override async Task HandleAsync(UpdateProductRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_PRODUCTS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var existingProduct = await shopDbContext.Products.FindAsync(ProductId.Of(req.ProductId), ct);

        if (existingProduct is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        var photoUrl = string.Empty;
        if (req.ImageFile != null)
        {
            //TODO: Upload the image to a storage service and get the URL
        }

        existingProduct.Update(
            name: req.Name,
            barcodeNumber: req.BarcodeNumber ?? existingProduct.BarcodeNumber ?? string.Empty,
            photoUrl: photoUrl,
            categoryId: CategoryId.Of(req.CategoryId),
            description: req.Description,
            lowThresholdValue: req.LowThresholdValue);
        
        await shopDbContext.SaveChangesAsync(ct);

        await SendOkAsync(ct);
    }
}

public class UpdateProductRequestValidator : Validator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Product name is required");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category id is required");
    }
}