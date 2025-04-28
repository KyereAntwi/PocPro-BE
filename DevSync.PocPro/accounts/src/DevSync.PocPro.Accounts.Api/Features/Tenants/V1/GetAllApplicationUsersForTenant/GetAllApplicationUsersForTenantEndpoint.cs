namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetAllApplicationUsersForTenant;

public class GetAllApplicationUsersForTenantEndpoint(IApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor) 
    : Endpoint<GetAllApplicationUsersForTenantRequest, BaseResponse<IEnumerable<GetApplicationUsersForTenantResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/{Id}/users");
        Description(x => x
            .WithName("GetUsers")
            .Produces<BaseResponse<IEnumerable<GetApplicationUsersForTenantResponse>>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(GetAllApplicationUsersForTenantRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existingUser = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.UserId == userId, ct);

        if (existingUser is null || !existingUser.HasPermission(PermissionType.VIEW_USERS))
        {
            await SendUnauthorizedAsync(ct);
        }
        
        var tenant = await applicationDbContext.Tenants.FindAsync(TenantId.Of(req.Id), ct);
        if (tenant is null)
        {
            await SendNotFoundAsync(ct);
        }
        
        var applicationUsers = await applicationDbContext.ApplicationUsers
            .Where(a => a.TenantId == TenantId.Of(req.Id))
            .Select(a => new GetApplicationUsersForTenantResponse(
                a.FirstName,
                a.LastName,
                a.Email ?? string.Empty,
                a.OtherNames ?? string.Empty,
                a.PhotoUrl ?? string.Empty))
            .ToListAsync(ct);

        await SendOkAsync(new BaseResponse<IEnumerable<GetApplicationUsersForTenantResponse>>("Users fetched successfully", true)
        {
            Data = applicationUsers
        }, cancellation: ct);
    }
}

public class GetAllApplicationUsersForTenantRequestValidator : Validator<GetAllApplicationUsersForTenantRequest>
{
    public GetAllApplicationUsersForTenantRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotNull();
    }
}