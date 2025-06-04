namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.UpdateCustomer;

public record UpdateCustomerRequest([FromRoute] Guid Id, string FullName, string Email, string Phone, string Address, string Status);