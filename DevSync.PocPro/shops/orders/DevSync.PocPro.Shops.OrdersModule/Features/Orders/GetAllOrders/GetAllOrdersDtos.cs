namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetAllOrders;

public record GetAllOrdersRequest(
    string? OrderNumber,
    string? OrderType,
    string? CreatedFrom,
    string? CreatedTo,
    Guid? CustomerId,
    Guid? PosSessionId,
    string? Status,
    int Page = 1,
    int PageSize = 20);

public record GetAllOrdersResponse(IEnumerable<OrderItemDto> Items);
    
public record OrdersResponse(
    Guid Id, 
    string OrderType,
    string Status,
    string OrderNumber,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy);