namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetAllOrders;

public record GetAllOrdersRequest(
    string OrderNumber = "",
    string OrderType = "",
    string CreatedFrom = "",
    string CreatedTo = "",
    string Customer = "",
    string OrderStatus = "",
    string PosSession = "",
    string Status = "",
    int Page = 1,
    int PageSize = 20);
    
public record OrdersResponse(
    Guid Id, 
    string OrderType,
    string Status,
    string OrderStatus,
    string OrderNumber,
    DateTimeOffset? CreatedAt,
    DateTimeOffset? UpdatedAt,
    string? CreatedBy);