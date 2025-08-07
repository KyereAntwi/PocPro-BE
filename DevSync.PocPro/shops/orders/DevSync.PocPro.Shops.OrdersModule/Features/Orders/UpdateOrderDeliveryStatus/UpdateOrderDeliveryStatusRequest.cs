namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.UpdateOrderDeliveryStatus;

public record UpdateOrderDeliveryStatusRequest(
    Guid Id,
    string NewStatus);