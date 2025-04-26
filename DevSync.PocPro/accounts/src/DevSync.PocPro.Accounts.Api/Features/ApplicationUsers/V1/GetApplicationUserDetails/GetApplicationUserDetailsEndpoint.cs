namespace DevSync.PocPro.Accounts.Api.Features.ApplicationUsers.V1.GetApplicationUserDetails;

public class GetApplicationUserDetailsEndpoint : EndpointWithoutRequest<GetApplicationUserDetailsResponse>
{
    public override void Configure()
    {
        Get("/api/v1/accounts/users/{Id}");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        return base.HandleAsync(ct);
    }
}