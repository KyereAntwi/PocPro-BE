using DevSync.PocPro.Shops.Shared.Interfaces;
using FluentValidation;

namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrder;

public class CreateOrderEndpoint(
    IOrderModuleDbContext orderModuleDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPurchaseServices purchaseServices) 
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
                PermissionType.MANAGE_SALES, userId!),
            OrderType.OnlineOrder => true,
            _ => false
        };

        if (!hasPermission)
        {
            await SendAsync(
                new BaseResponse<Guid>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }

        List<OrderItem> orderItems = [];
        orderItems.AddRange(req.OrderItems.Select(item => new OrderItem(item.ProductId, item.Quantity)));

        var shippingAddress = req.ShippingAddress != null ? new ShippingAddress(
            req.ShippingAddress.AddressLine1,
            req.ShippingAddress.AddressLine2,
            req.ShippingAddress.City,
            Enum.Parse<Region>(req.ShippingAddress.Region),
            req.ShippingAddress.FullName,
            req.ShippingAddress.PhoneNumber
        ) : null;

        var attemptPurchaseRequest = 
            req.OrderItems.Select(item => new MakePurchaseOnProductsRequest(item.ProductId, item.Quantity, req.PosId)).ToList();

        var attemptPurchaseOnProductsResult = await purchaseServices.MakePurchaseOnProducts(attemptPurchaseRequest, ct);

        if (!attemptPurchaseOnProductsResult.IsSuccess)
        {
            await SendAsync(
                new BaseResponse<Guid>("Order Failed", false)
                {
                    Errors = attemptPurchaseOnProductsResult.Errors.Select(e => e.Message)
                },
                StatusCodes.Status422UnprocessableEntity, ct);
            return;
        }

        var newOrder = Order.Create(
            orderType: type, 
            orderItems: orderItems, 
            paymentMethod: Enum.Parse<PaymentMethod>(req.PaymentMethod), 
            shippingAddress: shippingAddress, 
            posSessionId: string.IsNullOrWhiteSpace(req.PosSessionId) ? Guid.Empty : Guid.Parse(req.PosSessionId),
            customerId: string.IsNullOrWhiteSpace(req.CustomerId) ? Guid.Empty : Guid.Parse(req.CustomerId),
            amountReceived: req.AmountReceived);

        if (newOrder.IsFailed)
        {
            await SendAsync(new BaseResponse<Guid>("Bad Request", false)
            {
                Errors = newOrder.Errors.Select(e => e.Message)
            },StatusCodes.Status400BadRequest, ct);
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

public class CreateOrderRequestValidator: Validator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.OrderType)
            .Must(x => Enum.TryParse<OrderType>(x, true, out _))
            .WithMessage("Order type must be a valid type");

        RuleFor(x => x.PaymentMethod)
            .Must(x => Enum.TryParse<PaymentMethod>(x, true, out _))
            .WithMessage("Payment must be a valid Payment method");

        RuleFor(x => x.PosId)
            .NotEmpty().WithMessage("Pos Id is required")
            .NotNull();

        RuleFor(x => x.OrderItems)
            .Must(list => list == null || list.All(item => item.ProductId != Guid.Empty && item.Quantity > 0))
            .WithMessage("There should be at least 1 Order Item with Quantity above 0");
        
        RuleFor(x => x.ShippingAddress)
            .Cascade(CascadeMode.Stop)
            .Must((request, address) =>
            {
                if (Enum.TryParse<OrderType>(request.OrderType, true, out var orderType) && orderType == OrderType.OnlineOrder)
                {
                    return address != null
                           && !string.IsNullOrWhiteSpace(address.FullName)
                           && !string.IsNullOrWhiteSpace(address.PhoneNumber)
                           && !string.IsNullOrWhiteSpace(address.AddressLine1);
                }
                return true;
            }).WithMessage("FullName, PhoneNumber, and AddressLine1 are required for OnlineOrder.");
    }
}