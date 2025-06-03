namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrder;

public class CreateOrderEndpoint(
    IOrderModuleDbContext orderModuleDbContext, IHttpContextAccessor httpContextAccessor, ITenantServices tenantServices) 
    : Endpoint<CreateOrderRequest, BaseResponse<Guid>>
{
    public override void Configure()
    {
        Post("/api/v1/orders");
    }

    public override async Task HandleAsync(CreateOrderRequest req, CancellationToken ct)
    {
        var type = Enum.Parse<OrderType>(req.OrderType);
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var hasPermission = type switch
        {
            OrderType.PurchaseOrder => await tenantServices.UserHasRequiredPermissionAsync(
                PermissionType.MANAGE_PURCHASES, userId!),
            OrderType.SalesOrder => await tenantServices.UserHasRequiredPermissionAsync(
                PermissionType.MANAGE_SALES,
                userId!),
            OrderType.OnlineOrder => true,
            _ => false
        };

        if (!hasPermission)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        List<OrderItem> orderItems = [];
        orderItems.AddRange(req.OrderItems.Select(item => new OrderItem(item.ProductId, item.Quantity)));

        var shippingAddress = new ShippingAddress(
            req.ShippingAddress.AddressLine1,
            req.ShippingAddress.AddressLine2,
            req.ShippingAddress.City,
            Enum.Parse<Region>(req.ShippingAddress.Region),
            req.ShippingAddress.FullName,
            req.ShippingAddress.PhoneNumber
        );
        
        var newOrder = Order.Create(
            type, 
            orderItems, 
            Enum.Parse<PaymentMethod>(req.PaymentMethod), 
            shippingAddress, 
            req.PosSessionId == Guid.Empty ? Guid.Empty : req.PosSessionId,
            req.CustomerId == Guid.Empty ? Guid.Empty : req.CustomerId);

        if (newOrder.IsFailed)
        {
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        await orderModuleDbContext.Orders.AddAsync(newOrder.Value, ct);
        await orderModuleDbContext.SaveChangesAsync(ct);

        await SendCreatedAtAsync<GetOrderEndpoint>(new
        {
            Id = newOrder.Value.Id.Value
        }, new BaseResponse<Guid>("Order created successfully", true)
        {
            Data = newOrder.Value.Id.Value
        }, cancellation: ct);
    }
}