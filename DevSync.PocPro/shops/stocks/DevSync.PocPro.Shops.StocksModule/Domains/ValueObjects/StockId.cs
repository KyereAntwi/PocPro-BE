namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record StockId
{
    public Guid Value { get; } = Guid.Empty;
    
    private StockId(Guid value) => Value = value;

    public static StockId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for StockId");
        }
        
        return new StockId(value);
    }
}