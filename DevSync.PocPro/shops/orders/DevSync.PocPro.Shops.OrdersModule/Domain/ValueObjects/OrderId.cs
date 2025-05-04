namespace DevSync.PocPro.Shops.OrdersModule.Domain.ValueObjects;

public record OrderId
{
    public Guid Value { get; } = Guid.Empty;
    
    private OrderId(Guid value) => Value = value;

    public static OrderId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for OrderId");
        }
        
        return new OrderId(value);
    }
}