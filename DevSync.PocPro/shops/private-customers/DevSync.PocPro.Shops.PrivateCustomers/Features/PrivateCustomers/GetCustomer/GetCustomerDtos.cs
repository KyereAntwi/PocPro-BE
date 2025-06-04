namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.GetCustomer;

public record GetCustomerRequest([FromRoute] Guid Id);

public record GetCustomerResponse(Guid Id, string FullName, string Email, string Phone, string Address, string Status);