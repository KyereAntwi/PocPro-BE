namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrder;

public record CreateOrderRequest(
    IEnumerable<OrderItemRequest> OrderItems,
    ShippingAddressRequest? ShippingAddress,
    string PaymentMethod,
    string OrderType,
    string PosSessionId,
    string CustomerId,
    double AmountReceived,
    Guid PosId);

public record OrderItemRequest(Guid ProductId, int Quantity);

public record ShippingAddressRequest(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Region,
    string Country
);