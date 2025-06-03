namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record CategoryId
{
    public Guid Value { get; } = Guid.Empty;
    
    private CategoryId(Guid value) => Value = value;

    public static CategoryId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for ContactId");
        }
        
        return new CategoryId(value);
    }
}