namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class GetAllTenantsQueryHandler(
    IApplicationDbContext applicationDbContext)
    : IQueryHandler<GetAllTenantsQuery, List<GetTenantDetailsResponse>>
{
    public async Task<Result<List<GetTenantDetailsResponse>>> HandleAsync(GetAllTenantsQuery command, CancellationToken cancellationToken = default)
    {
        var allTenants = await applicationDbContext
            .Tenants
            .Select(t => new GetTenantDetailsResponse(
                t.ConnectionString,
                t.Id.Value,
                t.UniqueIdentifier,
                t.SubscriptionType.ToString(),
                null))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Ok(allTenants);
    }
}