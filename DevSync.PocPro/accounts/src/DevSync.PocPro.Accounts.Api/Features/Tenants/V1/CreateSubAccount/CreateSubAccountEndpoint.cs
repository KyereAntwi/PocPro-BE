using DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;
using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetSubAccountDetails;

namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount;

public class CreateSubAccountEndpoint(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext applicationDbContext,
    PaymentService.PaymentServiceClient paymentServiceClient,
    ApiAuthentication apiAuthentication,
    ILogger<CreateSubAccountEndpoint> logger)
    : Endpoint<CreateSubAccountRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/accounts/tenants/sub-account");
        Description(x => x
            .WithName("CreateSubAccount")
            .Produces<BaseResponse<Guid>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override async Task HandleAsync(CreateSubAccountRequest req, CancellationToken ct)
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

        if (tenant is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        try
        {
            var createSubAccountRequest = new Grpc.CreateSubAccountRequest
            {
                BusinessName = req.BusinessName,
                SettlementBank = req.SettlementBank,
                AccountNumber = req.AccountNumber,
                TenantIdentifier = tenant.UniqueIdentifier,
                PercentageCharge = 100,
                App = "AccountApi"
            };

            var metadata = new Metadata
            {
                { "x-api-key", apiAuthentication.ApiKey }
            };

            var response =
                await paymentServiceClient.CreateSubAccountAsync(createSubAccountRequest, metadata,
                    cancellationToken: ct);

            if (!response.IsSuccess)
            {
                await SendErrorsAsync((int)HttpStatusCode.BadRequest, ct);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating sub-account for tenant {TenantId}. Error = {Error}",
                tenant.Id, e.Message);
            await SendErrorsAsync((int)HttpStatusCode.InternalServerError, ct);
            return;
        }

        tenant.UpdateSubAccount(new SubAccount(
            tenant.Id,
            req.BusinessName,
            req.SettlementBank,
            req.AccountNumber,
            100), userId!);

        applicationDbContext.Tenants.Update(tenant);
        await applicationDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetSubAccountDetailsEndpoint>(
            new { },
            new BaseResponse<Guid>("Sub-account created successfully.", true) { Data = tenant.SubAccount!.Id.Value },
            cancellation: ct);
    }
}

public class CreateSubAccountValidator : Validator<CreateSubAccountRequest>
{
    public CreateSubAccountValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty().WithMessage("Business name is required.")
            .NotNull();
        
        RuleFor(x => x.SettlementBank)
            .NotEmpty().WithMessage("Settlement bank is required.")
            .NotNull();

        RuleFor(x => x.AccountNumber)
            .NotEmpty().WithMessage("Account number is required.")
            .NotNull();
    }
}