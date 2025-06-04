namespace DevSync.PocPro.Shops.OrdersModule.Domain;

public class OrderItem : BaseEntity<OrderItemId>
{
    private OrderItem() { }

    internal OrderItem(Guid productId, int quantity)
    {
        Id = OrderItemId.Of(Guid.CreateVersion7());
        ProductId = productId;
        Quantity = quantity;
        Status = StatusType.Active;
    }

    public OrderId OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
}