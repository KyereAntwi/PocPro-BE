namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.GetCustomer;

public class GetCustomerEndpoint(
    ICustomerDbContext customerDbContext, ITenantServices tenantServices, IHttpContextAccessor httpContextAccessor) 
    : Endpoint<GetCustomerRequest, BaseResponse<GetCustomerResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/customers/{Id}");
    }

    public override async Task HandleAsync(GetCustomerRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermissions = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_CUSTOMERS, userId);
        if (!hasRequiredPermissions)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var customer = await customerDbContext.Customers.FindAsync(CustomerId.Of(req.Id), ct);

        if (customer is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendOkAsync(new BaseResponse<GetCustomerResponse>("Customer fetched successfully.", true)
        {
            Data = new GetCustomerResponse(customer.Id.Value, customer.FullName, customer.Email ?? "")
        }, cancellation: ct);
    }
}