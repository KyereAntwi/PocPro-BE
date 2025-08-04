namespace DevSync.PocPro.Shops.OrdersModule.Features.Orders.CreateOrdersInBulk;

public record CreateOrdersInBulkRequest
{
    public IEnumerable<CreateOrderRequest> Orders { get; set; } = new List<CreateOrderRequest>();
}