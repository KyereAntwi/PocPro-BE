namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.GetSupplier;

public record GetSupplierRequest(Guid Id);

public record GetSupplierResponse(Guid Id, string Title, string Email, IEnumerable<ContactResponse> Contacts, DateTimeOffset? CreatedAt, DateTimeOffset? UpdatedAt, string Status);

public record ContactResponse(Guid Id, string Person, string Value, string ContactType);