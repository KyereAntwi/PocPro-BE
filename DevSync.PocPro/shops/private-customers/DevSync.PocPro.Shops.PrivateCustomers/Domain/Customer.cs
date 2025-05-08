namespace DevSync.PocPro.Shops.PrivateCustomers.Domain;

public class Customer : BaseEntity<CustomerId>
{
    public static Customer Add(string fullName, string email)
    {
        var customer = new Customer
        {
            Id = CustomerId.Of(Guid.CreateVersion7()),
            FullName = fullName,
            Email = email
        };
        
        return customer;
    }

    public void Update(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
    }
    
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
}