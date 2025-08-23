namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetSubAccountDetails;

public class GetSubAccountDetailsEndpoint(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext applicationDbContext) 
    : EndpointWithoutRequest<BaseResponse<GetSubAccountDetailsResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/sub-account/details");
        Description(x => x
            .WithName("GetSubAccountDetails")
            .Produces<BaseResponse<GetSubAccountDetailsResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingUser = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(a => a.UserId == userId)
            .FirstOrDefaultAsync(ct);

        if (existingUser is null || !existingUser.HasPermission(PermissionType.MANAGE_SUBACCOUNTS))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var tenant = await applicationDbContext.Tenants
            .Include(t => t.SubAccount)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == existingUser.TenantId, ct);

        if (tenant?.SubAccount is null)
        {
            await SendErrorsAsync((int)HttpStatusCode.NotFound, ct);
            return;
        }
        
        var response = new GetSubAccountDetailsResponse(
            tenant.SubAccount.BusinessName,
            tenant.SubAccount.SettlementBank,
            tenant.SubAccount.AccountNumber,
            tenant.SubAccount.PercentageCharge);
        
        await SendOkAsync(new BaseResponse<GetSubAccountDetailsResponse>("Sub-account fetched successfully", true)
        {
            Data = response
        }, ct);
    }
}