using DevSync.PocPro.Shops.StocksModule.Features.Products.GetProductDetails;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.AddProduct;

public class AddProductEndpoint(
    IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<AddProductRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/products");
    }

    public override async Task HandleAsync(AddProductRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_PRODUCTS, userId!))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var photoUrl = string.Empty;
        if (req.ImageFile != null)
        {
            //TODO: Upload the image to a storage service and get the URL
        }

        var product = Product.Create(
            req.Name, req.BarcodeNumber!, photoUrl, CategoryId.Of(req.CategoryId), req.Description, req.LowThresholdValue);
        await shopDbContext.Products.AddAsync(product, ct);
        await shopDbContext.SaveChangesAsync(ct);
        
        await SendCreatedAtAsync<GetProductDetailsEndpoint>(new
        {
            ProductId = product.Id.Value,
        },
            new BaseResponse<Guid>("Product added", true)
            {
                Data = product.Id.Value,
            }, cancellation: ct);
    }
}

public class AddProductRequestValidator : Validator<AddProductRequest>
{
    public AddProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).WithMessage("Product name is required");
        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category id is required");
    }
}