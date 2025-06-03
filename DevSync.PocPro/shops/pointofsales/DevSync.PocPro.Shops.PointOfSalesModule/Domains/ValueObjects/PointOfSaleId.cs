namespace DevSync.PocPro.Shops.PointOfSales.Domains.ValueObjects;

public record PointOfSaleId
{
    public Guid Value { get; } = Guid.Empty;
    
    private PointOfSaleId(Guid value) => Value = value;

    public static PointOfSaleId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainExceptions("Invalid value for PointOfSaleId");
        }
        
        return new PointOfSaleId(value);
    }
}