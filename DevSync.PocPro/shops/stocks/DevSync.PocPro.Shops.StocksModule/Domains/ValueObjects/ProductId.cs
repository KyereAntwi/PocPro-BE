namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record ProductId
{
    public Guid Value { get; } = Guid.Empty;
    
    private ProductId(Guid value) => Value = value;

    public static ProductId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for ProductId");
        }
        
        return new ProductId(value);
    }
}