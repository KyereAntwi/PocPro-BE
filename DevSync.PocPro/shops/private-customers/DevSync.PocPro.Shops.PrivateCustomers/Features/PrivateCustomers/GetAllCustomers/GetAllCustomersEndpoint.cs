using DevSync.PocPro.Shops.Shared.Interfaces;

namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.GetAllCustomers;

public class GetAllCustomersEndpoint(
    ICustomerDbContext customerDbContext, ITenantServices tenantServices, IHttpContextAccessor httpContextAccessor)
    : EndpointWithoutRequest<BaseResponse<IEnumerable<GetCustomerResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/customers");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermissions = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_CUSTOMERS, userId);
        if (!hasRequiredPermissions)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var customers = await customerDbContext
            .Customers
            .Select(c => 
                new GetCustomerResponse(
                    c.Id.Value, 
                    c.FullName, 
                    c.Email ?? string.Empty,
                    c.Phone ?? string.Empty,
                    c.Address ?? string.Empty,
                    c.Status.ToString() ?? StatusType.Active.ToString()))
            .AsNoTracking()
            .ToListAsync(ct);
        
        await SendOkAsync(new BaseResponse<IEnumerable<GetCustomerResponse>>("Customers fetched successfully", true)
        {
            Data = customers
        }, cancellation: ct);
    }
}