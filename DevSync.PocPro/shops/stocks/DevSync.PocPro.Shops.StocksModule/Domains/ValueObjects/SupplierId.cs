namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record SupplierId
{
    public Guid Value { get; } = Guid.Empty;
    
    private SupplierId(Guid value) => Value = value;

    public static SupplierId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for SupplierId");
        }
        
        return new SupplierId(value);
    }
}