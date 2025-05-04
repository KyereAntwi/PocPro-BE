namespace DevSync.PocPro.Shops.OrdersModule.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    InProgress = 2,
    Transit = 3,
    Delivered = 4,
    Cancelled = 5,
    Returned = 6
}