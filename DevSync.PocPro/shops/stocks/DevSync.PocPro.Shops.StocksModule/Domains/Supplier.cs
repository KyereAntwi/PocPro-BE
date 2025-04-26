namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Supplier : BaseEntity<SupplierId>
{
    private readonly Collection<Contact> _contacts = [];
    public IReadOnlyCollection<Contact> Contacts => _contacts;
    
    public static Supplier Create(string title, Contact[]? contacts)
    {
        var supplier = new Supplier
        {
            Id = SupplierId.Of(Guid.CreateVersion7()),
            Title = title
        };

        if (contacts is not { Length: > 0 }) return supplier;
        
        foreach (var contact in contacts)
        {
            supplier._contacts.Add(contact);
        }

        return supplier;
    }

    public string Title { get; private set; } = string.Empty;
}