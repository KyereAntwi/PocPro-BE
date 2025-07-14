using DevSync.PocPro.Shops.Shared.Utils;

namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetAllPOSForWorkerService;

public class GetAllPOSForWorkerServiceEndpoint(
    IPOSDbContext dbContext, IHttpContextAccessor httpContextAccessor, ApiAuthentication authentication) 
    : EndpointWithoutRequest<IEnumerable<Guid>>
{
    public override void Configure()
    {
        Get("api/v1/pointofsales/get-all/for-service-worker");
        AllowAnonymous();
        Description(x => x
            .WithTags("PointOfSalesIdsForWorkerService")
            .Produces<IEnumerable<Guid>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var apiToken = httpContextAccessor.HttpContext?.Request.Headers["X-Api-Token"].ToString();
        
        if (string.IsNullOrEmpty(apiToken))
        {
            await SendUnauthorizedAsync(cancellation: ct);
            return;
        }

        if (!apiToken.Equals(authentication.ApiKey))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var posIds = await dbContext
            .PointOfSales
            .Where(p => p.Status == StatusType.Active)
            .Select(x => x.Id.Value)
            .ToListAsync(ct);

        await SendOkAsync(posIds, ct);
    }
}