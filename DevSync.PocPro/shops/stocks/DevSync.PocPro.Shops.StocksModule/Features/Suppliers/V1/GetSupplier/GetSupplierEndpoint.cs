namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.GetSupplier;

public class GetSupplierEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetSupplierRequest, BaseResponse<GetSupplierResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/suppliers/{Id}");
    }

    public override async Task HandleAsync(GetSupplierRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_SUPPLIERS, userId);

        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var supplier = await shopDbContext
            .Suppliers
            .Include(s => s.Contacts)
            .FirstOrDefaultAsync(s => s.Id == SupplierId.Of(req.Id), ct);

        if (supplier == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetSupplierResponse>("Supplier fetched successfully.", true)
        {
            Data = new GetSupplierResponse(
                supplier.Id.Value,
                supplier.Title,
                supplier.Contacts
                    .Select(c => new ContactResponse(c.Id.Value, c.Value, c.ContactType.ToString())))
        }, cancellation: ct);
    }
}