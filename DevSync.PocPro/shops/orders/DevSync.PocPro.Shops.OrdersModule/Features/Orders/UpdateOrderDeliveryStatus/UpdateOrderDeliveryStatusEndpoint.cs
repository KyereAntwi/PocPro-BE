namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.UpdateOrderDeliveryStatus;

public class UpdateOrderDeliveryStatusEndpoint(
    IOrderModuleDbContext orderModuleDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPurchaseServices purchaseServices)
    : Endpoint<UpdateOrderDeliveryStatusRequest>
{
    public override void Configure()
    {
        Put("/api/v1/orders/{Id}");
    }

    public override async Task HandleAsync(UpdateOrderDeliveryStatusRequest req, CancellationToken ct)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var hasPermission = false;
        
        hasPermission = await tenantServices.UserHasRequiredPermissionAsync(
            PermissionType.MANAGE_PURCHASES, userId!);

        if (!hasPermission)
        {
            hasPermission = await tenantServices.UserHasRequiredPermissionAsync(
                PermissionType.MANAGE_SALES, userId!);
        }


        if (!hasPermission)
        {
            await SendAsync(
                new BaseResponse<IEnumerable<Guid>>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }
        
        var existingOrder = await orderModuleDbContext
            .Orders
            .Include(o => o.OrderItems)
            .Where(o => o.Id == OrderId.Of(req.Id))
            .AsSplitQuery()
            .FirstOrDefaultAsync(ct);

        if (existingOrder == null)
        {
            await SendAsync(
                new BaseResponse<IEnumerable<Guid>>("Order not found", false)
                {
                    Errors = ["Order not found"]
                },
                StatusCodes.Status404NotFound, ct);
            return;
        }
        
        var newStatus = Enum.Parse<OrderStatus>(req.NewStatus, true);
        var response = existingOrder.UpdateOrderDeliveryStatus(newStatus);

        if (response.IsFailed)
        {
            await SendAsync(
                new BaseResponse<IEnumerable<Guid>>("Process failed", false)
                {
                    Errors = response.Errors.Select(e => e.Message)
                },
                StatusCodes.Status400BadRequest, ct);
        }
        
        orderModuleDbContext.Orders.Update(existingOrder);
        await orderModuleDbContext.SaveChangesAsync(ct);
        
        if (newStatus == OrderStatus.Cancelled)
        {
            await purchaseServices.ReversePurchaseOnProducts(
                existingOrder
                    .OrderItems
                    .Select(item => new MakePurchaseOnProductsRequest(
                    item.ProductId, 
                    item.Quantity,
                    existingOrder.PointOfSaleId!.Value))
                    .ToList(), ct);
        }
        
        await SendNoContentAsync(ct);
    }
}

public class UpdateOrderDeliveryStatusRequestValidator : Validator<UpdateOrderDeliveryStatusRequest>
{
    public UpdateOrderDeliveryStatusRequestValidator()
    {
        RuleFor(x => x.NewStatus)
            .Must(x => Enum.TryParse<OrderStatus>(x, true, out _))
            .WithMessage("New status must be a valid status");
    }
}