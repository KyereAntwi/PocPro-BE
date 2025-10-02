namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.CheckExistingUniqueIdentifier;

public class CheckExistingUniqueIdentifierEndpoint(
    IQueryHandler<CheckExistingUniqueIdentifierQuery, CheckExistingUniqueIdentifierResponse> handler) 
    : Endpoint<CheckExistingUniqueIdentifierRequest, BaseResponse<CheckExistingUniqueIdentifierResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/check-existing-unique-identifier/{UniqueIdentifier}");
    }

    public override async Task HandleAsync(CheckExistingUniqueIdentifierRequest req, CancellationToken ct)
    {
        var exists = await handler.HandleAsync(new CheckExistingUniqueIdentifierQuery(req.UniqueIdentifier), ct);
        await SendOkAsync(new BaseResponse<CheckExistingUniqueIdentifierResponse>("Existing unique identifier",true)
        {
            Data = new CheckExistingUniqueIdentifierResponse(exists.Value.UniqueIdentifierExists),
        }, ct);
    }
}