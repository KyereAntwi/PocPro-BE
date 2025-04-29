namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.GetSupplier;

public record GetSupplierRequest(Guid Id);

public record GetSupplierResponse(Guid Id, string Title, IEnumerable<ContactResponse> Contacts);

public record ContactResponse(Guid Id, string Value, string ContactType);