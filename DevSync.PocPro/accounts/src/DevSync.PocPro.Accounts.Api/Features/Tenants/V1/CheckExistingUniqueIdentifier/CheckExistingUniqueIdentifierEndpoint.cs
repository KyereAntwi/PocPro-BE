namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckExistingUniqueIdentifier;

public class CheckExistingUniqueIdentifierEndpoint(IApplicationDbContext applicationDbContext) 
    : Endpoint<CheckExistingUniqueIdentifierRequest, BaseResponse<CheckExistingUniqueIdentifierResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/check-existing-unique-identifier/{UniqueIdentifier}");
    }

    public override async Task HandleAsync(CheckExistingUniqueIdentifierRequest req, CancellationToken ct)
    {
        var exists = await applicationDbContext.Tenants.Where(t => t.UniqueIdentifier == req.UniqueIdentifier)
            .FirstOrDefaultAsync(ct);
        
        await SendOkAsync(new BaseResponse<CheckExistingUniqueIdentifierResponse>("Existing unique identifier",true)
        {
            Data = new CheckExistingUniqueIdentifierResponse(exists != null),
        }, ct);
    }
}