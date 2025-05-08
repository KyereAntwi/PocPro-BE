using DevSync.PocPro.Shared.Domain.Events;

namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.DeleteCustomer;

public class DeleteCustomerEndpoint(
    ICustomerDbContext customerDbContext, 
    ITenantServices tenantServices, 
    IHttpContextAccessor httpContextAccessor,
    ILogger<DeleteCustomerEndpoint> logger,
    IPublishEndpoint publishEndpoint) 
    : Endpoint<DeleteCustomerRequest>
{
    public override void Configure()
    {
        Delete("/api/v1/customers/{Id}");
    }

    public override async Task HandleAsync(DeleteCustomerRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermissions = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_CUSTOMERS, userId);
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
        
        customerDbContext.Customers.Remove(customer);
        await customerDbContext.SaveChangesAsync(ct);

        try
        {
            await publishEndpoint.Publish(new DeleteCustomerEvent
            {
                CustomerId = customer.Id.Value
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError("Error sending delete customer event for customer {Id}. Error = {Error}", req.Id, e.Message);
        }

        await SendNoContentAsync(ct);
    }
}