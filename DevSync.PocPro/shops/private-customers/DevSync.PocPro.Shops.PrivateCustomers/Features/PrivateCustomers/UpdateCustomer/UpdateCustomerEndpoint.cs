namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.UpdateCustomer;

public class UpdateCustomerEndpoint(
    ICustomerDbContext customerDbContext, ITenantServices tenantServices, IHttpContextAccessor httpContextAccessor) 
    : Endpoint<UpdateCustomerRequest>
{
    public override void Configure()
    {
        Put("/api/customers/{Id:guid}");
    }

    public override async Task HandleAsync(UpdateCustomerRequest req, CancellationToken ct)
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
        
        customer.Update(req.FullName, req.Email);
        
        await customerDbContext.SaveChangesAsync(ct);

        await SendNoContentAsync(ct);
    }
}

public class UpdateCustomerRequestValidator : Validator<UpdateCustomerRequest>
{
    public UpdateCustomerRequestValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().WithMessage("Please specify full name").NotNull();
    }
}