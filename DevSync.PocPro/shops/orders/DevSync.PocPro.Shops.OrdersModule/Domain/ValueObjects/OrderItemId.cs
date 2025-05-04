namespace DevSync.PocPro.Shops.OrdersModule.Domain.ValueObjects;

public record OrderItemId
{
    public Guid Value { get; } = Guid.Empty;
    
    private OrderItemId(Guid value) => Value = value;

    public static OrderItemId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for OrderItemId");
        }
        
        return new OrderItemId(value);
    }
}