namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrderByOrderNumber;

public class GetOrderByOrderNumberEndpoint(IOrderModuleDbContext orderModuleDbContext) 
    : Endpoint<GetOrderByOrderNumberRequest, BaseResponse<GetOrderResponse>>
{
    public override void Configure()
    {
        Get("/api/v1/orders/by-order-number/{OrderNumber}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(GetOrderByOrderNumberRequest req, CancellationToken ct)
    {
        GetOrderResponse? order = order = await orderModuleDbContext
            .Orders
            .Where(o => o.OrderNumber == req.OrderNumber)
            .Select(o => new GetOrderResponse(
                o.Id.Value,
                o.OrderItems.Select(item =>
                    new OrderItemDto(
                        item.Id.Value,
                        item.ProductId,
                        item.Quantity,
                        item.ProductPrice)),
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
                o.PointOfSaleId == null ? null : o.PointOfSaleId.Value,
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