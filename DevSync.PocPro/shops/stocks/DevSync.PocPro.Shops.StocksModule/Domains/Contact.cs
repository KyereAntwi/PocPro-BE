namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Contact : BaseEntity<ContactId>
{
    private Contact() {}

    internal Contact(SupplierId supplierId, string value, ContactType type, string person)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 10)
        {
            throw new DomainExceptions("Contact value must be at least 10 characters long");
        }

        if (string.IsNullOrWhiteSpace(person))
        {
            throw new DomainExceptions("Contact Person must not be null");
        }

        Id = ContactId.Of(Guid.CreateVersion7());
        SupplierId = supplierId;
        Value = value;
        ContactType = type;
        Person = person;
    }

    public SupplierId SupplierId { get; set; }
    public string Person { get; set; }
    public string Value { get; private set; }
    public ContactType ContactType { get; private set; }
}