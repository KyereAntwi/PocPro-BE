namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.AddSupplier;

public class AddSupplierEndpoint(IShopDbContext shopDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<AddSupplierRequest, BaseResponse<Guid>>
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

        var supplier = Supplier.Create(req.Title, req.Email);

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
            new BaseResponse<Guid>("Supplier added", true)
            {
                Data = supplier.Id.Value
            }, cancellation: ct);
    }
}

public class AddSupplierRequestValidator: Validator<AddSupplierRequest>
{
    public AddSupplierRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Invalid email address.");

        When(x => x.Contacts != null && x.Contacts.Any(), () =>
        {
            RuleForEach(x => x.Contacts).ChildRules(contact =>
            {
                contact.RuleFor(c => c.Person)
                    .NotEmpty().WithMessage("Contact person is required.");
                contact.RuleFor(c => c.Value)
                    .NotEmpty().WithMessage("Contact value is required.");
                contact.RuleFor(c => c.ContactType)
                    .Must(type => Enum.TryParse<ContactType>(type, out _))
                    .WithMessage("Invalid contact type.");
            });
        });
    }
}

