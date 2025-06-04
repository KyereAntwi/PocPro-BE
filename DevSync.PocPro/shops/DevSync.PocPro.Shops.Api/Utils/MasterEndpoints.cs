namespace DevSync.PocPro.Shops.Api.Utils;

public class MasterEndpoints(IMasterExtensions masterExtensions) 
    : EndpointWithoutRequest<bool>
{
    public override void Configure()
    {
        Get("/api/v1/master/run-migrations");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            await masterExtensions.ApplyMigrationsAsync(ct);
            await SendOkAsync(true, ct);
        }
        catch (Exception e)
        {
            await SendErrorsAsync(cancellation: ct);
        }
        
    }
}