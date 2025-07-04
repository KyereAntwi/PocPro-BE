using DevSync.PocPro.Shops.Shared.Interfaces;

namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetAllOrders;

public class GetAllOrdersEndpoint(
    IOrderModuleDbContext orderModuleDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices) 
    : Endpoint<GetAllOrdersRequest, BaseResponse<PagedResponse<OrdersResponse>>>
{
    public override void Configure()
    {
        Get("/api/v1/orders");
    }

    public override async Task HandleAsync(GetAllOrdersRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_ORDERS, userId!);

        if (!hasRequiredPermission)
        {
            await SendAsync(
                new BaseResponse<PagedResponse<OrdersResponse>>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                }, StatusCodes.Status403Forbidden, ct);
            return;
        }

        var orderQuery = orderModuleDbContext.Orders.AsNoTracking();

        orderQuery = FilterOrderList(req, orderQuery);
        
        var totalCount = await orderQuery.LongCountAsync(ct);
        var pagedList = await orderQuery
            .Select(order => new OrdersResponse(
                order.Id.Value,
                order.Type.ToString(),
                order.Status.ToString() ?? StatusType.Active.ToString(),
                order.OrderStatus.ToString(),
                order.OrderNumber,
                order.CreatedAt,
                order.UpdatedAt,
                order.CreatedBy))
            .Skip((req.Page - 1) * req.PageSize)
            .Take(req.PageSize)
            .ToListAsync(ct);
        
        var response = new PagedResponse<OrdersResponse>(req.Page, req.PageSize, totalCount, pagedList);
        
        await SendOkAsync(new BaseResponse<PagedResponse<OrdersResponse>>("Orders fetched successfully.", true)
        {
            Data = response
        }, cancellation: ct);
    }

    private static IQueryable<Order> FilterOrderList(
        GetAllOrdersRequest req, IQueryable<Order> orderQuery)
    {
        if (!string.IsNullOrWhiteSpace(req.OrderType))
        {
            var type = Enum.Parse<OrderType>(req.OrderType);
            orderQuery = orderQuery.Where(order => order.Type == type);
        }

        if (!string.IsNullOrWhiteSpace(req.Customer))
        {
            orderQuery = orderQuery.Where(order => order.CustomerId == CustomerId.Of(Guid.Parse(req.Customer)));
        }

        if (!string.IsNullOrWhiteSpace(req.PosSession))
        {
            orderQuery = orderQuery.Where(order => order.PosSessionId == SessionId.Of(Guid.Parse(req.PosSession)));
        }

        if (!string.IsNullOrWhiteSpace(req.CreatedFrom))
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt!.Value.Date >= DateTime.Parse(req.CreatedFrom).Date);
        }
        
        if (!string.IsNullOrWhiteSpace(req.CreatedTo))
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt!.Value.Date <= DateTime.Parse(req.CreatedTo).Date);
        }

        if (!string.IsNullOrWhiteSpace(req.OrderStatus))
        {
            var status = Enum.Parse<OrderStatus>(req.OrderStatus);
            orderQuery = orderQuery.Where(order => order.OrderStatus == status);
        }
        
        if (!string.IsNullOrWhiteSpace(req.Status))
        {
            var status = Enum.Parse<StatusType>(req.Status);
            orderQuery = orderQuery.Where(order => order.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(req.OrderNumber))
        {
            orderQuery = orderQuery.Where(order => order.OrderNumber.ToLower().Contains(req.OrderNumber.ToLower()));
        }
        
        return orderQuery;
    }
}