namespace DevSync.PocPro.Shops.PrivateCustomers.Domain;

public class Customer : BaseEntity<CustomerId>
{
    public static Customer Add(string fullName, string email, string phone, string address)
    {
        var customer = new Customer
        {
            Id = CustomerId.Of(Guid.CreateVersion7()),
            FullName = fullName,
            Email = email,
            Phone = phone,
            Address = address,
            Status = StatusType.Active
        };
        
        return customer;
    }

    public void Update(string fullName, string email, string phone, string address, StatusType status)
    {
        FullName = fullName;
        Email = email;
        Phone = phone;
        Address = address;
        Status = status;
    }
    
    public string FullName { get; private set; } = string.Empty;
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
}