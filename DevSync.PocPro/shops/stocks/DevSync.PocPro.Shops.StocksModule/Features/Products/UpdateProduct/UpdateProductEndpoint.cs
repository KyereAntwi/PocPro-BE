using DevSync.PocPro.Shared.Domain.Utils;
using DevSync.PocPro.Shops.Shared.Events;

namespace DevSync.PocPro.Shops.StocksModule.Features.Products.UpdateProduct;

public class UpdateProductEndpoint (
    IShopDbContext shopDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    RabbitMqConnectionFactory rabbitMqConnection,
    ILogger<UpdateProductEndpoint> logger)
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

        existingProduct.Update(
            name: req.Name,
            barcodeNumber: req.BarcodeNumber ?? existingProduct.BarcodeNumber ?? string.Empty,
            photoUrl: req.PhotoUrl ?? string.Empty,
            categoryId: CategoryId.Of(req.CategoryId),
            description: req.Description,
            lowThresholdValue: req.LowThresholdValue,
            isFeatured: req.IsFeatured,
            brandId: req.BrandId != Guid.Empty ? BrandId.Of(req.BrandId) : existingProduct.BrandId);
        
        await shopDbContext.SaveChangesAsync(ct);

        try
        {
            var tenant = await tenantServices.GetTenantByUserIdAsync(userId!);
            
            var updateEvent = new ProductUpdatedEvent
            {
                ProductId = req.ProductId,
                Name = req.Name,
                BarcodeNumber = req.BarcodeNumber ?? existingProduct.BarcodeNumber ?? string.Empty,
                PhotoUrl = req.PhotoUrl ?? string.Empty,
                IsFeatured = req.IsFeatured,
                Identifier = tenant!.UniqueIdentifier
            };
            
            var publisher = new EventPublisher(rabbitMqConnection);
            await publisher.PublishAsync(updateEvent, "shop_exchange", "update_product", ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error publishing product updated event: {Message}", e.Message);
        }

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