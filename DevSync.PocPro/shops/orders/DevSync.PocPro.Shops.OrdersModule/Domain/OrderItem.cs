namespace DevSync.PocPro.Shops.OrdersModule.Domain;

public class OrderItem : BaseEntity<OrderItemId>
{
    private OrderItem() { }

    internal OrderItem(Guid productId, int quantity, decimal productPrice)
    {
        Id = OrderItemId.Of(Guid.CreateVersion7());
        ProductId = productId;
        Quantity = quantity;
        Status = StatusType.Active;
        ProductPrice = productPrice;
    }

    public OrderId OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal ProductPrice { get; private set; }
}