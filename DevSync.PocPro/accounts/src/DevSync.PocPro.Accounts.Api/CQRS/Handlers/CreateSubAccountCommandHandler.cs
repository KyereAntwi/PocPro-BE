using DevSync.PocPro.Accounts.Api.Features.Tenants.Grpc;
using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount;
using CreateSubAccountRequest = DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CreateSubAccount.CreateSubAccountRequest;

namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class CreateSubAccountCommandHandler(
    IHttpContextAccessor httpContextAccessor,
    IApplicationDbContext applicationDbContext,
    PaymentService.PaymentServiceClient paymentServiceClient,
    ApiAuthentication apiAuthentication,
    ILogger<CreateSubAccountEndpoint> logger) : Shared.Domain.CQRS.ICommandHandler<CreateSubAccountRequest, Guid>
{
    public async Task<Result<Guid>> HandleAsync(CreateSubAccountRequest command, CancellationToken cancellationToken = default)
    {
        var userId = httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingUser = await applicationDbContext
            .ApplicationUsers
            .Include(a => a.Permissions)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(a => a.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingUser is null || !existingUser.HasPermission(PermissionType.MANAGE_SUBACCOUNTS))
        {
            return Result.Fail("Forbidden");
        }
        
        var tenant = await applicationDbContext.Tenants
            .Include(t => t.SubAccount)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == existingUser.TenantId, cancellationToken);

        if (tenant is null)
        {
            return Result.Fail("Tenant not found");
        }
        
        try
        {
            var grpcRequest = new Features.Tenants.Grpc.CreateSubAccountRequest
            {
                BusinessName = command.BusinessName,
                SettlementBank = command.SettlementBank,
                AccountNumber = command.AccountNumber,
                TenantIdentifier = tenant.UniqueIdentifier,
            };
            
            var metadata = new Metadata
            {
                { "x-api-key", apiAuthentication.ApiKey }
            };

            var response =
                await paymentServiceClient.CreateSubAccountAsync(grpcRequest, metadata,
                    cancellationToken: cancellationToken);

            if (!response.IsSuccess)
            {
                logger.LogError("Failed to create sub-account for tenant {TenantId}. Error = {Error}",
                    tenant.Id, response.ErrorMessage);
                return Result.Fail("Failed to create sub-account");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while creating sub-account for tenant {TenantId}. Error = {Error}",
                tenant.Id, e.Message);
            return Result.Fail("An error occurred while creating sub-account");
        }
        
        tenant.UpdateSubAccount(new SubAccount(
            tenant.Id,
            command.BusinessName,
            command.SettlementBank,
            command.AccountNumber,
            100), userId!);

        applicationDbContext.Tenants.Update(tenant);
        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return Result.Ok(tenant.SubAccount!.Id.Value);
    }
}