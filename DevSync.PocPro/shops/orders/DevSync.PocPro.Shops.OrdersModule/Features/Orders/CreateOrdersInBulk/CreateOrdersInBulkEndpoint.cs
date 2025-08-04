namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrdersInBulk;

public class CreateOrdersInBulkEndpoint(
    IOrderModuleDbContext dbContext,
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPurchaseServices purchaseServices)
    : Endpoint<CreateOrdersInBulkRequest, BaseResponse<IEnumerable<string>>>
{
    public override void Configure()
    {
        Post("/api/v1/orders/bulk");
    }

    public override async Task HandleAsync(CreateOrdersInBulkRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var hasPermission = await tenantServices.UserHasRequiredPermissionAsync(
            PermissionType.MANAGE_SALES, userId!);
        
        if (!hasPermission)
        {
            await SendAsync(
                new BaseResponse<IEnumerable<string>>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        List<Order> orders = [];
        
        foreach (var order in req.Orders)
        {
            List<OrderItem> orderItems = [];
            orderItems.AddRange(order.OrderItems.Select(item => new OrderItem(item.ProductId, item.Quantity, item.Price)));
            
            var attemptPurchaseRequest = 
                order.OrderItems.Select(item => new MakePurchaseOnProductsRequest(
                        item.ProductId, 
                        item.Quantity, 
                        order.PosId)).ToList();
            
            var attemptPurchaseOnProductsResult = await purchaseServices.MakePurchaseOnProducts(attemptPurchaseRequest, ct);
            
            if (!attemptPurchaseOnProductsResult.IsSuccess)
            {
                await SendAsync(
                    new BaseResponse<IEnumerable<string>>("Order Failed", false)
                    {
                        Errors = attemptPurchaseOnProductsResult.Errors.Select(e => e.Message)
                    },
                    StatusCodes.Status422UnprocessableEntity, ct);
                return;
            }
            
            var newOrder = Order.Create(
                orderType: Enum.Parse<OrderType>(order.OrderType), 
                orderItems: orderItems, 
                paymentMethod: Enum.Parse<PaymentMethod>(order.PaymentMethod), 
                shippingAddress: null, 
                posSessionId: string.IsNullOrWhiteSpace(order.PosSessionId) ? Guid.Empty : Guid.Parse(order.PosSessionId),
                customerId: string.IsNullOrWhiteSpace(order.CustomerId) ? Guid.Empty : Guid.Parse(order.CustomerId),
                pointOfSaleId: order.PosId,
                amountReceived: order.AmountReceived,
                orderNumber: order.OrderNumber);
            
            if (newOrder.IsFailed)
            {
                await SendAsync(new BaseResponse<IEnumerable<string>>("Bad Request", false)
                {
                    Errors = newOrder.Errors.Select(e => e.Message)
                },StatusCodes.Status400BadRequest, ct);
                return;
            }
            
            orders.Add(newOrder.Value);
        }

        if (orders.Count > 0)
        {
            await dbContext.Orders.AddRangeAsync(orders, ct);
            await dbContext.SaveChangesAsync(ct);
        }
        
        await SendOkAsync(new BaseResponse<IEnumerable<string>>("Bulk order created successfully", true)
        {
            Data = orders.Select(o => o.OrderNumber).ToList()
        }, ct);
    }
}

public class CreateOrdersInBulkRequestValidator : Validator<CreateOrdersInBulkRequest>
{
    public CreateOrdersInBulkRequestValidator()
    {
        RuleFor(x => x.Orders)
            .NotEmpty()
            .WithMessage("At least one order is required.");

        RuleForEach(x => x.Orders)
            .SetValidator(new CreateOrderRequestValidator());
    }
}