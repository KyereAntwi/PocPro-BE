namespace DevSync.PocPro.Shops.OrdersModule.Domain.ValueObjects;

public record ShippingAddressId
{
    public Guid Value { get; } = Guid.Empty;
    
    private ShippingAddressId(Guid value) => Value = value;

    public static ShippingAddressId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for ShippingAddressId");
        }
        
        return new ShippingAddressId(value);
    }
}