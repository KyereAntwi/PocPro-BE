namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.AddSupplier;

public class AddSupplierEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<AddSupplierRequest, BaseResponse<AddSupplierResponse>>
{
    public override void Configure()
    {
        Post("/api/v1/suppliers");
    }

    public override async Task HandleAsync(AddSupplierRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_SUPPLIERS, userId);

        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
        }

        var supplier = Supplier.Create(req.Title);

        if (req.Contacts.Any())
        {
            var actualContacts = req
                .Contacts
                .Select(c => new Contact(supplier.Id, c.Value, Enum.Parse<ContactType>(c.ContactType))).ToArray();
            
            supplier.AddContacts(actualContacts);
        }

        await shopDbContext.Suppliers.AddAsync(supplier, ct);
        await shopDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetSupplierEndpoint>(
            new { Id = supplier.Id }, 
            new BaseResponse<AddSupplierResponse>("Supplier added", true)
            {
                // Data = new AddSupplierResponse(
                //     supplier.Id.Value, 
                //     supplier.Title, 
                //     supplier.Contacts.Select(c => new ContactResponse(c.Id.Value, c.Value, c.ContactType.ToString())).ToArray()
            }, cancellation: ct);
    }
}

public class AddSupplierRequestValidator: Validator<AddSupplierRequest>
{
    public AddSupplierRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");
    }
}