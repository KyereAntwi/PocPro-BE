namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.CreatePOS;

public class CreatePOSEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<CreatePOSRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/pointofsales");
    }

    public override async Task HandleAsync(CreatePOSRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var newPOS = PointOfSale.Create(
            req.Title, 
            req.Phone ?? string.Empty, 
            req.Address ?? string.Empty, 
            req.Email ?? string.Empty);
        
        await posDbContext.PointOfSales.AddAsync(newPOS, ct);
        await posDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetPOSEndpoint>(new
            {
                Id = newPOS.Id.Value
            }, new BaseResponse<Guid>("POS created successfully", true)
            {
                Data = newPOS.Id.Value
            },
            cancellation: ct);
    }
}

public class CreatePOSRequestValidator : Validator<CreatePOSRequest>
{
    public CreatePOSRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .NotNull();
        
        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Provided Email is not valid.");
        
        RuleFor(x => x.Phone)
            .Matches(@"^\+?[0-9]{10,15}$")
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Provided Phone number is not valid.");
    }
}