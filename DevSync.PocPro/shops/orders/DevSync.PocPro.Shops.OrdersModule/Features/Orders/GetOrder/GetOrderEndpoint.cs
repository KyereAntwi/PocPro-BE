using DevSync.PocPro.Shops.Shared.Interfaces;

namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrder;

public class GetOrderEndpoint (
    IOrderModuleDbContext orderModuleDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices)
    : Endpoint<GetOrderRequest, BaseResponse<GetOrderResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/orders/{Id}");
    }

    public override async Task HandleAsync(GetOrderRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        var hasRequiredPermission = await tenantServices.UserHasRequiredPermissionAsync(PermissionType.VIEW_ORDERS, userId!);

        if (!hasRequiredPermission)
        {
            await SendAsync(new BaseResponse<GetOrderResponse>("Permission Denied", false)
            {
                Errors = ["You do not have required permission"]
            }, StatusCodes.Status403Forbidden, ct);
            return;
        }

        GetOrderResponse? order = order = await orderModuleDbContext
            .Orders
            .Where(o => o.Id == OrderId.Of(req.Id))
            .Select(o => new GetOrderResponse(
                o.Id.Value,
                o.OrderItems.Select(item =>
                    new OrderItemDto(
                        item.Id.Value,
                        item.ProductId,
                        item.Quantity)),
                o.Type.ToString(),
                o.OrderStatus.ToString(),
                o.Status.ToString() ?? StatusType.Active.ToString(),
                o.OrderNumber,
                o.ShippingAddress == null ? null : new ShippingAddressDto(
                    o.ShippingAddress.ContactName,
                    o.ShippingAddress.ContactPhone,
                    o.ShippingAddress.Address1,
                    o.ShippingAddress.Address2 ?? string.Empty,
                    o.ShippingAddress.City ?? string.Empty,
                    o.ShippingAddress.Region.ToString() ?? string.Empty),
                o.PosSessionId == null ? null : o.PosSessionId.Value,
                o.CustomerId == null ? null : o.CustomerId.Value,
                o.CreatedAt,
                o.UpdatedAt,
                o.CreatedBy ?? string.Empty))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (order is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendOkAsync(new BaseResponse<GetOrderResponse>("Order retrieved successfully", true)
        {
            Data = order
        }, ct);
    }
}