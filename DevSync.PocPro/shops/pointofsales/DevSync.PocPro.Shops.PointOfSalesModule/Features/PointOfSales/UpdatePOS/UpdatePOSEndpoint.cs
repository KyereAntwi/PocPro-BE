namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.UpdatePOS;

public class UpdatePOSEndpoint (
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : Endpoint<UpdatePOSDto>
{
    public override void Configure()
    {
        Put("/api/v1/pointofsales/{Id}");
    }

    public override async Task HandleAsync(UpdatePOSDto req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.MANAGE_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var pos = await posDbContext.PointOfSales
            .FindAsync(PointOfSaleId.Of(req.Id), ct);

        if (pos is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        pos.Update(
            req.Title, 
            req.Phone ?? pos.Phone ?? string.Empty, 
            req.Address ?? pos.Address ?? string.Empty, 
            req.Email ?? pos.Email ?? string.Empty,
            !string.IsNullOrWhiteSpace(req.Status) ? Enum.Parse<StatusType>(req.Status!) : pos.Status);
        
        await posDbContext.SaveChangesAsync(ct);
        
        await SendNoContentAsync(ct);
    }
}

public class UpdatePOSDtoValidator : Validator<UpdatePOSDto>
{
    public UpdatePOSDtoValidator()
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