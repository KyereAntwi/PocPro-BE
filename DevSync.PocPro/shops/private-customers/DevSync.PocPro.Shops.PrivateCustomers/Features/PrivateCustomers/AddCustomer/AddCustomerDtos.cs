namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.AddCustomer;

public record AddCustomerRequest(string FullName, string Email);

public record AddCustomerResponse(Guid CustomerId);