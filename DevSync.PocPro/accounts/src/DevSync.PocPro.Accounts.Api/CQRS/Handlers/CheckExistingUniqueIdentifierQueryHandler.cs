using DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckExistingUniqueIdentifier;

namespace DevSync.PocPro.Accounts.Api.CQRS.Handlers;

public class CheckExistingUniqueIdentifierQueryHandler(
    IApplicationDbContext applicationDbContext)
    : IQueryHandler<CheckExistingUniqueIdentifierQuery, CheckExistingUniqueIdentifierResponse>
{
    public async Task<Result<CheckExistingUniqueIdentifierResponse>> HandleAsync(CheckExistingUniqueIdentifierQuery query, CancellationToken cancellationToken = default)
    {
        var exists = await applicationDbContext
            .Tenants
            .Where(t => t.UniqueIdentifier == query.UniqueIdentifier)
            .FirstOrDefaultAsync(cancellationToken);

        return Result.Ok(exists is null ? new CheckExistingUniqueIdentifierResponse(false) : new CheckExistingUniqueIdentifierResponse(true));
    }
}