namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.GetCustomer;

public record GetCustomerRequest([FromRoute] Guid Id);

public record GetCustomerResponse(Guid Id, string FullName, string Email);