namespace DevSync.PocPro.Shops.PointOfSales.Domains.ValueObjects;

public record PosManagerId
{
    public Guid Value { get; } = Guid.Empty;
    
    private PosManagerId(Guid value) => Value = value;

    public static PosManagerId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for PointOfSaleManagerId");
        }
        
        return new PosManagerId(value);
    }
}