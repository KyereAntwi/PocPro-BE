namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetAllPOS;

public class GetAllPOSEndpoint(
    IPOSDbContext pocContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices)
    : Endpoint<GetAllPOSRequest, BaseResponse<GetAllPOSResponse>>
{
    public override void Configure()
    {
        Get("/pointofsales");
    }

    public override async Task HandleAsync(GetAllPOSRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.GET_POS, userId!);
        
        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var pos = pocContext
            .PointOfSales
            .Include(p => p.Managers)
            .AsSplitQuery()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(req.Keyword))
        {
            pos = pos.Where(p => p.Title.ToLower().Contains(req.Keyword.ToLower()));
        }
        
        if (!string.IsNullOrEmpty(req.UserId))
        {
            pos = pos
                .Where(p => p.Managers.Any(m => m.UserId == req.UserId));
        }

        var response = new BaseResponse<GetAllPOSResponse>("", true)
        {
            Data = new GetAllPOSResponse(await pos.Select(x => 
                    new POSResponse(
                    x.Id.Value,
                    x.Title,
                    x.Email ?? string.Empty,
                    x.Address ?? string.Empty,
                    x.Phone ?? string.Empty,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.UpdatedAt,
                    x.UpdatedBy))
                .ToListAsync(ct))
        };
        
        await SendOkAsync(response, cancellation: ct);
    }
}