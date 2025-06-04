namespace DevSync.PocPro.Shops.OrdersModule.Domain;

public class ShippingAddress : BaseEntity<ShippingAddressId>
{
    private ShippingAddress() {}

    internal ShippingAddress(string address1, string address2, string city, Region region, string contactName, string contactPhone)
    {
        Id = ShippingAddressId.Of(Guid.CreateVersion7());
        Address1 = address1;
        Address2 = address2;
        City = city;
        Region = region;
        ContactName = contactName;
        ContactPhone = contactPhone;
        Status = StatusType.Active;
    }
    
    public OrderId OrderId { get; private set; }
    public string Address1 { get; private set; } = string.Empty;
    public string? Address2 { get; private set; }
    public string? City { get; private set; }
    public Region? Region { get; private set; }
    public string ContactName { get; private set; } = string.Empty;
    public string ContactPhone { get; private set; } = string.Empty;
}