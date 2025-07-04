namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record BrandId
{
    public Guid Value { get; } = Guid.Empty;
    
    private BrandId(Guid value) => Value = value;

    public static BrandId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for BrandId");
        }
        
        return new BrandId(value);
    }
}