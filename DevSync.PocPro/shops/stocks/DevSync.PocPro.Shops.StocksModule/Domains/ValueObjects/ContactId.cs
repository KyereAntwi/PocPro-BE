namespace DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;

public record ContactId
{
    public Guid Value { get; } = Guid.Empty;
    
    private ContactId(Guid value) => Value = value;

    public static ContactId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for ContactId");
        }
        
        return new ContactId(value);
    }
}