namespace DevSync.PocPro.Shops.PointOfSales.Features.PointOfSales.GetSessions;

public class GetSessionsEndpoint(
    IPOSDbContext posDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<GetSessionsRequest, BaseResponse<PagedResponse<SessionResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/pointofsales/{Id}/sessions");
    }

    public override async Task HandleAsync(GetSessionsRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor?.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_SESSIONS, userId!);

        if (!hasRequiredPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var query = posDbContext.Sessions.Where(s => s.PointOfSaleId == PointOfSaleId.Of(req.Id)).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(req.DateFrom))
        {
            query = query.Where(s => s.CreatedAt >= DateTime.Parse(req.DateFrom));
        }

        if (!string.IsNullOrWhiteSpace(req.DateTo))
        {
            query = query.Where(s => s.CreatedAt <= DateTime.Parse(req.DateTo));
        }
        
        var total = await query.LongCountAsync(ct);

        var pagedList = await query
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SessionResponse(
                s.Id.Value,
                s.OpeningCash,
                s.ClosingCash,
                s.CreatedAt,
                s.ClosedAt,
                s.ClosedBy,
                s.CreatedBy))
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync(ct);
        
        var response = new PagedResponse<SessionResponse>(req.Page, req.PageSize, total, pagedList);
        
        await SendOkAsync(new BaseResponse<PagedResponse<SessionResponse>>("Sessions", true)
        {
            Data = response
        }, cancellation: ct);
    }
}