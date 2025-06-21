namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record ProductMediaId
{
    public Guid Value { get; } = Guid.Empty;
    
    private ProductMediaId(Guid value) => Value = value;

    public static ProductMediaId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for Product Media Id");
        }
        
        return new ProductMediaId(value);
    }
}