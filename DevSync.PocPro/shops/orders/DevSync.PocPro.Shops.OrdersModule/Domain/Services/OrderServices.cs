namespace DevSync.PocPro.Shops.OrdersModule.Domain.Services;

public static class OrderServices
{
    public static string GenerateOrderNumber()
    {
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
        return orderNumber;
    }
}