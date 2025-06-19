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
            await SendForbiddenAsync(ct);
            return;
        }
        
        var order = await orderModuleDbContext
            .Orders
            .Select(order => new GetOrderResponse(
                order.Id.Value,
                order.OrderItems.Select(item => 
                    new OrderItemDto(
                        item.Id.Value, 
                        item.ProductId,
                        item.Quantity)),
                order.Type.ToString(),
                order.OrderStatus.ToString(),
                order.Status.ToString() ?? StatusType.Active.ToString(),
                order.OrderNumber,
                order.ShippingAddress == null ? null : new ShippingAddressDto(
                    order.ShippingAddress.ContactName,
                    order.ShippingAddress.ContactPhone,
                    order.ShippingAddress.Address1,
                    order.ShippingAddress.Address2,
                    order.ShippingAddress.City,
                    order.ShippingAddress.Region.ToString()),
                order.PosSessionId!.Value,
                order.CustomerId!.Value,
                order.CreatedAt,
                order.UpdatedAt,
                order.CreatedBy ?? string.Empty))
            .AsNoTracking()
            .FirstOrDefaultAsync(order => order.Id == req.Id, ct);

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