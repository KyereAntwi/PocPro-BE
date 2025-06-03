namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.AddCustomer;

public class AddCustomerEndpoint(
    ICustomerDbContext customerDbContext, ITenantServices tenantServices, IHttpContextAccessor httpContextAccessor) 
    : Endpoint<AddCustomerRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/customers");
    }

    public override async Task HandleAsync(AddCustomerRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermissions = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_CUSTOMERS, userId);
        if (!hasRequiredPermissions)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var customer = Customer.Add(req.FullName, req.Email);
        
        await customerDbContext.Customers.AddAsync(customer, ct);
        await customerDbContext.SaveChangesAsync(ct);
        
        await SendCreatedAtAsync<GetCustomerEndpoint>(
            new { customer.Id}, 
            new BaseResponse<Guid>("Added Customer Successfully", true)
            {
                Data = customer.Id.Value
            },
            cancellation: ct);
    }
}

public class AddCustomerRequestValidator : Validator<AddCustomerRequest>
{
    public AddCustomerRequestValidator()
    {
        RuleFor(c => c.FullName).NotEmpty().WithMessage("Please specify full name").NotNull();
    }
}