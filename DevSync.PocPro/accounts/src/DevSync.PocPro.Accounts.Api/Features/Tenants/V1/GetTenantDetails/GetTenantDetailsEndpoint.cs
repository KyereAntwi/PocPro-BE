namespace DevSync.PocPro.Accounts.Api.Features.Tenants.V1.GetTenantDetails;

public class GetTenantDetailsEndpoint : Endpoint<GetTenantDetailsRequest, GetTenantDetailsResponse>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/tenants/{UserId}");
        Description(x => x
            .WithName("GetTenantDetails")
            .Produces<BaseResponse<GetTenantDetailsResponse>>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound));
    }

    public override Task HandleAsync(GetTenantDetailsRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}