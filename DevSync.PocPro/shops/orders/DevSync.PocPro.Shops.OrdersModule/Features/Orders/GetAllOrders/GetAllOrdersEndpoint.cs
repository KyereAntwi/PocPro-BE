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
            await SendForbiddenAsync(ct);
            return;
        }

        var orderQuery = orderModuleDbContext.Orders.AsNoTracking();

        orderQuery = FilterOrderList(req, orderQuery);
        
        var totalCount = await orderModuleDbContext.Orders.CountAsync(ct);
        var pagedList = await orderQuery
            .Select(order => new OrdersResponse(
                order.Id.Value,
                order.Type.ToString(),
                order.Status.ToString(),
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

        if (req.CustomerId.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.CustomerId == CustomerId.Of(req.CustomerId.Value));
        }

        if (req.PosSessionId.HasValue)
        {
            orderQuery = orderQuery.Where(order => order.PosSessionId == SessionId.Of(req.PosSessionId.Value));
        }

        if (!string.IsNullOrWhiteSpace(req.CreatedFrom))
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt!.Value.Date >= DateTime.Parse(req.CreatedFrom).Date);
        }
        
        if (!string.IsNullOrWhiteSpace(req.CreatedTo))
        {
            orderQuery = orderQuery.Where(order => order.CreatedAt!.Value.Date <= DateTime.Parse(req.CreatedTo).Date);
        }

        if (!string.IsNullOrWhiteSpace(req.Status))
        {
            var status = Enum.Parse<OrderStatus>(req.Status);
            orderQuery = orderQuery.Where(order => order.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(req.OrderNumber))
        {
            orderQuery = orderQuery.Where(order => order.OrderNumber.ToLower().Contains(req.OrderNumber.ToLower()));
        }
        
        return orderQuery;
    }
}