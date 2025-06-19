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
        Guid customerId = default,
        string? customerName = null,
        double amountReceived = 0)
    {
        if (orderItems.Count == 0)
        {
            return Result.Fail("Order must have at least one item");
        }
        
        switch (orderType)
        {
            case OrderType.SalesOrder when posSessionId == Guid.Empty:
                return Result.Fail("Sales order must have a POS Session ID");
            case OrderType.PurchaseOrder when customerId  == Guid.Empty:
                return Result.Fail("Purchase order must have a Customer ID");
            case OrderType.OnlineOrder:
                break;
        }

        var newOrder = new Order
        {
            Id = OrderId.Of(Guid.CreateVersion7()),
            Type = orderType,
            OrderStatus = orderType == OrderType.OnlineOrder ? OrderStatus.Pending : OrderStatus.Delivered,
            PosSessionId = posSessionId == Guid.Empty ? null : SessionId.Of(posSessionId),
            CustomerId = customerId == Guid.Empty ? null : CustomerId.Of(posSessionId),
            PaymentMethod = paymentMethod ?? PaymentMethod.Cash,
            OrderNumber = OrderServices.GenerateOrderNumber(),
            CustomerName = customerName,
            AmountReceived = amountReceived,
            Status = StatusType.Active
        };

        foreach (var orderItem in orderItems)
        {
            newOrder._orderItems.Add(orderItem);
        }

        if (shippingAddress != null)
        {
            newOrder.ShippingAddress = shippingAddress;
        }
        
        return Result.Ok(newOrder);
    }

    public Result UpdateOrderDeliveryStatus(OrderStatus status)
    {
        if (status == OrderStatus.Cancelled && OrderStatus != OrderStatus.Pending)
        {
            return Result.Fail("Only pending orders can be cancelled");
        }

        OrderStatus = status;
        return Result.Ok();
    }

    public string OrderNumber { get; private set; } = string.Empty;
    public OrderType Type { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public SessionId? PosSessionId { get; private set; }
    public CustomerId? CustomerId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? CustomerName { get; set; }
    public double AmountReceived { get; set; }
}