namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrder;

public record GetOrderRequest(Guid Id);

public record GetOrderResponse(
    IEnumerable<OrderItemDto> OrderItems,
    string OrderType,
    string Status,
    string OrderNumber,
    ShippingAddressDto? ShippingAddress,
    Guid? PosSessionId);

public record OrderItemDto(Guid Id, ProductDto Product, int Quantity);

public record ProductDto(Guid Id, string Name, decimal Price, string? PhotoUrl);

public record ShippingAddressDto(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? Region);