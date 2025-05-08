namespace DevSync.PocPro.Shops.PrivateCustomers.Features.PrivateCustomers.GetAllCustomers;

public record GetAllCustomersResponse(IEnumerable<GetCustomerResponse> Customers);