namespace DevSync.PocPro.Shops.StocksModule.Domains;

public class Supplier : BaseEntity<SupplierId>
{
    private readonly Collection<Contact> _contacts = [];
    public IReadOnlyCollection<Contact> Contacts => _contacts;
    
    public static Supplier Create(string title, string? email)
    {
        var supplier = new Supplier
        {
            Id = SupplierId.Of(Guid.CreateVersion7()),
            Title = title,
            Email = email
        };

        return supplier;
    }

    public void AddContacts(Contact[] contacts)
    {
        foreach (var contact in contacts)
        {
            _contacts.Add(contact);
        }
    }

    public string Title { get; private set; } = string.Empty;
    public string? Email { get; set; }
}