namespace DevSync.PocPro.Shops.OrdersModule.Domain;

public class Order : BaseEntity<OrderId>
{
    private Collection<OrderItem> _orderItems = [];
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public ShippingAddress? ShippingAddress { get; private set; }
    
    public static Result<Order> Create(
        OrderType orderType,
        List<OrderItem> orderItems,
        PaymentMethod? paymentMethod,
        ShippingAddress? shippingAddress = null,
        Guid posSessionId = default,
        Guid customerId = default)
    {
        if (orderItems.Count == 0)
        {
            return Result.Fail("Order must have at least one item");
        }
        
        if(orderType == OrderType.SalesOrder && posSessionId == Guid.Empty)
        {
            return Result.Fail("Sales order must have a POS Session ID");
        }

        if (orderType == OrderType.PurchaseOrder && customerId  == Guid.Empty)
        {
            return Result.Fail("Purchase order must have a Customer ID");
        }
        
        var newOrder = new Order
        {
            Id = OrderId.Of(Guid.CreateVersion7()),
            Type = orderType,
            Status = orderType == OrderType.OnlineOrder ? OrderStatus.Pending : OrderStatus.Delivered,
            PosSessionId = posSessionId == Guid.Empty ? null : SessionId.Of(posSessionId),
            CustomerId = customerId == Guid.Empty ? null : CustomerId.Of(posSessionId),
            PaymentMethod = paymentMethod ?? PaymentMethod.Cash,
            OrderNumber = OrderServices.GenerateOrderNumber(),
        };

        foreach (var orderItem in orderItems)
        {
            newOrder._orderItems.Add(orderItem);
        }

        if (shippingAddress != null && orderType == OrderType.OnlineOrder)
        {
            newOrder.ShippingAddress = shippingAddress;
        }
        
        return Result.Ok(newOrder);
    }

    public Result UpdateStatus(OrderStatus status)
    {
        if (status == OrderStatus.Cancelled && Status != OrderStatus.Pending)
        {
            return Result.Fail("Only pending orders can be cancelled");
        }

        Status = status;
        return Result.Ok();
    }

    public string OrderNumber { get; private set; } = string.Empty;
    public OrderType Type { get; private set; }
    public OrderStatus Status { get; private set; }
    public SessionId? PosSessionId { get; private set; }
    public CustomerId? CustomerId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
}