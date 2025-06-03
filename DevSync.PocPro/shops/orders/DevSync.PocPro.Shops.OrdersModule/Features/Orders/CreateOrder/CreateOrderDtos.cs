namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrder;

public record CreateOrderRequest(
    IEnumerable<OrderItemRequest> OrderItems,
    ShippingAddressRequest ShippingAddress,
    string PaymentMethod,
    string OrderType,
    Guid PosSessionId,
    Guid CustomerId);

public record OrderItemRequest(Guid ProductId, int Quantity);

public record CreateOrderResponse(Guid OrderId);

public record ShippingAddressRequest(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Region,
    string Country
);