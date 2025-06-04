namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrder;

public record GetOrderRequest(Guid Id);

public record GetOrderResponse(
    Guid Id,
    IEnumerable<OrderItemDto> OrderItems,
    string OrderType,
    string OrderStatus,
    string Status,
    string OrderNumber,
    ShippingAddressDto? ShippingAddress,
    Guid? PosSessionId,
    Guid? CustomerId,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    string CreatedBy);

public record OrderItemDto(Guid Id, Guid ProductId, int Quantity);

public record ProductDto(Guid Id, string Name, decimal Price, string? PhotoUrl);

public record ShippingAddressDto(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string? City,
    string? Region);