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
        
        var media = new List<ProductMedia>();
        if (req.Media != null)
        {
            media.AddRange(req.Media.Select(item => new ProductMedia(Enum.Parse<ProductMediaType>(item.MediaType), item.Url)));
        }

        var product = Product.Create(
            req.Name, req.BarcodeNumber!, req.PhotoUrl ?? string.Empty, CategoryId.Of(req.CategoryId), req.Description, req.LowThresholdValue, media);
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