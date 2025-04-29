namespace DevSync.PocPro.Shops.StocksModule.Features.Suppliers.V1.AddSupplier;

public record AddSupplierRequest(
    string Title,
    IEnumerable<ContactRequest> Contacts);

public record ContactRequest(string Value, string ContactType);

public record AddSupplierResponse(Guid Id);