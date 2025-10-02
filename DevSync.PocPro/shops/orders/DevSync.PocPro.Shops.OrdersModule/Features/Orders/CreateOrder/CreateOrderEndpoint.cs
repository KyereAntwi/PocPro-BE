namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrder;

public class CreateOrderEndpoint(
    IOrderModuleDbContext orderModuleDbContext, 
    IHttpContextAccessor httpContextAccessor, 
    ITenantServices tenantServices,
    IPurchaseServices purchaseServices,
    IPromoCodesServices promoCodesServices,
    IPaymentServices paymentServices) 
    : Endpoint<CreateOrderRequest, BaseResponse<IEnumerable<Guid>>>
{
    public override void Configure()
    {
        Post("/api/v1/orders");
        AllowAnonymous();
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
                new BaseResponse<IEnumerable<Guid>>("Permission Denied", false)
                {
                    Errors = ["You do not have required permission"]
                },
                StatusCodes.Status403Forbidden, ct);
            return;
        }

        if (!string.IsNullOrWhiteSpace(req.PaymentRef))
        {
            var paymentVerificationResult = await paymentServices.VerifyHttpAsync(req.PaymentRef, ct);
            if (!paymentVerificationResult)
            {
                await SendAsync(
                    new BaseResponse<IEnumerable<Guid>>("Order Failed", false)
                    {
                        Errors = ["Payment verification failed. Please check your payment reference."]
                    },
                    StatusCodes.Status400BadRequest, ct);
                return;
            }
        }

        List<OrderItem> orderItems = [];
        orderItems.AddRange(req.OrderItems.Select(item => new OrderItem(item.ProductId, item.Quantity, item.Price)));

        var shippingAddress = req.ShippingAddress != null ? new ShippingAddress(
            req.ShippingAddress.AddressLine1,
            req.ShippingAddress.AddressLine2,
            req.ShippingAddress.City,
            Enum.Parse<Region>(req.ShippingAddress.Region),
            req.ShippingAddress.FullName,
            req.ShippingAddress.PhoneNumber
        ) : null;

        if (!string.IsNullOrWhiteSpace(req.PromoCode) && !string.IsNullOrWhiteSpace(userId))
        {
            // validate promo code and apply it only when there is a userId - authenticated user
            var promoUseResponse = await promoCodesServices
                .UsePromoCodeAsync(req.PromoCode, userId, ct);

            if (!promoUseResponse)
            {
                await SendAsync(
                    new BaseResponse<IEnumerable<Guid>>("Order Failed", false)
                    {
                        Errors = ["Invalid, expired or used promo code"]
                    },
                    StatusCodes.Status400BadRequest, ct);
                return;
            }
        }

        var attemptPurchaseRequest = 
            req.OrderItems.Select(item => new MakePurchaseOnProductsRequest(
                item.ProductId, 
                item.Quantity, 
                type != OrderType.OnlineOrder 
                    ? req.PosId 
                    : item.PosId))
                .ToList();

        var attemptPurchaseOnProductsResult = await purchaseServices.MakePurchaseOnProducts(attemptPurchaseRequest, ct);
        if (!attemptPurchaseOnProductsResult.IsSuccess)
        {
            await SendAsync(
                new BaseResponse<IEnumerable<Guid>>("Order Failed", false)
                {
                    Errors = attemptPurchaseOnProductsResult.Errors.Select(e => e.Message)
                },
                StatusCodes.Status400BadRequest, ct);
            return;
        }

        List<Guid> posIdsFromLineItems = [];
        if (type == OrderType.OnlineOrder)
        {
            posIdsFromLineItems.AddRange(req.OrderItems.Select(item => item.PosId).Distinct());
        }
        else
        {
            posIdsFromLineItems.Add(req.PosId);       
        }

        List<Guid> response = [];

        var newOrder = new Result<Order>();

        if (type == OrderType.OnlineOrder)
        {
            foreach (var id in posIdsFromLineItems)
            {
                var productsIdsFromRequest = req
                    .OrderItems
                    .Where(item => item.PosId.Equals(id))
                    .Select(item => item.ProductId);
            
                newOrder  = Order.Create(
                    orderType: type, 
                    orderItems: orderItems.Where(p => productsIdsFromRequest.Contains(p.ProductId)).ToList(), 
                    paymentMethod: Enum.Parse<PaymentMethod>(req.PaymentMethod), 
                    shippingAddress: shippingAddress, 
                    posSessionId: string.IsNullOrWhiteSpace(req.PosSessionId) ? Guid.Empty : Guid.Parse(req.PosSessionId),
                    customerId: string.IsNullOrWhiteSpace(req.CustomerId) ? Guid.Empty : Guid.Parse(req.CustomerId),
                    pointOfSaleId: id,
                    amountReceived: req.AmountReceived,
                    orderNumber: req.OrderNumber,
                    promoCode: req.PromoCode);
            }
        }
        else
        {
            newOrder = Order.Create(
                orderType: type, 
                orderItems: orderItems, 
                paymentMethod: Enum.Parse<PaymentMethod>(req.PaymentMethod), 
                shippingAddress: shippingAddress, 
                posSessionId: string.IsNullOrWhiteSpace(req.PosSessionId) ? Guid.Empty : Guid.Parse(req.PosSessionId),
                customerId: string.IsNullOrWhiteSpace(req.CustomerId) ? Guid.Empty : Guid.Parse(req.CustomerId),
                pointOfSaleId: req.PosId,
                amountReceived: req.AmountReceived);
        }
        
        if (newOrder.IsFailed)
        {
            await SendAsync(new BaseResponse<IEnumerable<Guid>>("Bad Request", false)
            {
                Errors = newOrder.Errors.Select(e => e.Message)
            },StatusCodes.Status400BadRequest, ct);
            return;
        }
            
        response.Add(newOrder.Value.Id.Value);
        await orderModuleDbContext.Orders.AddAsync(newOrder.Value, ct);
        await orderModuleDbContext.SaveChangesAsync(ct);

        await SendOkAsync(new BaseResponse<IEnumerable<Guid>>("Order placed successfully", true)
        {
            Data = response
        }, ct);
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
            .NotNull()
            .When(x => Enum.TryParse<OrderType>(x.OrderType, true, out var orderType) && orderType != OrderType.OnlineOrder);

        RuleFor(x => x.OrderItems)
            .NotNull().WithMessage("Order Items are required")
            .Must(list => list.All(item => item.ProductId != Guid.Empty && item.Quantity > 0))
            .WithMessage("Each Order Item must have a valid ProductId and Quantity above 0");
        
        RuleFor(x => x.OrderItems)
            .Must(list => list.All(item => item.PosId != Guid.Empty))
            .WithMessage("All Order Items must have a POS ID")
            .When(x => Enum.TryParse<OrderType>(x.OrderType, true, out var orderType) && orderType == OrderType.OnlineOrder);
        
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
        
        RuleFor(x => x.ShippingAddress!.Region)
            .Must(region => Enum.TryParse<Region>(region, true, out _))
            .When(x => x.ShippingAddress != null)
            .WithMessage("Region must be a valid region type");
        
        RuleFor(x => x.PromoCode)
            .MaximumLength(50)
            .When(x => !string.IsNullOrWhiteSpace(x.PromoCode))
            .WithMessage("Promo code must not exceed 50 characters");

        RuleFor(x => x.PaymentRef)
            .NotEmpty().WithMessage("Payment ref should not be null or empty when order is an online order")
            .NotNull()
            .When(x => Enum.TryParse<OrderType>(x.OrderType, true, out var orderType) &&
                       orderType == OrderType.OnlineOrder);
    }
}