namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetAllOrders;

public record GetAllOrdersRequest(
    [Microsoft.AspNetCore.Mvc.FromQuery] string? OrderNumber,
    [Microsoft.AspNetCore.Mvc.FromQuery] string? OrderType,
    [Microsoft.AspNetCore.Mvc.FromQuery] string? CreatedFrom,
    [Microsoft.AspNetCore.Mvc.FromQuery] string? CreatedTo,
    [Microsoft.AspNetCore.Mvc.FromQuery] Guid? CustomerId,
    [Microsoft.AspNetCore.Mvc.FromQuery] Guid? PosSessionId,
    [Microsoft.AspNetCore.Mvc.FromQuery] string? Status,
    [Microsoft.AspNetCore.Mvc.FromQuery] int Page = 1,
    [Microsoft.AspNetCore.Mvc.FromQuery] int PageSize = 20);

public record GetAllOrdersResponse(IEnumerable<OrderItemDto> Items);
    
public record OrdersResponse(
    Guid Id, 
    string OrderType,
    string Status,
    string OrderNumber,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy);